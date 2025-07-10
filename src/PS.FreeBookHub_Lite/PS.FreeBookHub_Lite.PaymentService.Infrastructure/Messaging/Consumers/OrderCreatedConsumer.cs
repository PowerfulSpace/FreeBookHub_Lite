using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PS.FreeBookHub_Lite.PaymentService.Application.CQRS.Commands.ProcessPayment;
using PS.FreeBookHub_Lite.PaymentService.Application.Interfaces.Redis;
using PS.FreeBookHub_Lite.PaymentService.Common.Configuration;
using PS.FreeBookHub_Lite.PaymentService.Common.Events;
using PS.FreeBookHub_Lite.PaymentService.Common.Events.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Common.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics.Metrics;
using System.Text;
using System.Text.Json;


// 1) Получение сообщения – из очереди RabbitMQ.
// 2) Десериализация – в объект OrderCreatedEvent.
// 3) Проверка лимита повторных обработок – анализ заголовков (x-death).
// 4) Создание IServiceScope – для работы с DI.
// 5) Проверка дубликата – через IEventDeduplicationService.
// 6) Обработка платежа – отправка ProcessPaymentCommand через IMediator.
// 7) Публикация события PaymentCompletedEvent – через IEventPublisher.
// 8) Подтверждение обработки (BasicAck) – удаление сообщения из очереди.

namespace PS.FreeBookHub_Lite.PaymentService.Infrastructure.Messaging.Consumers
{
    public class OrderCreatedConsumer : BackgroundService
    {
        private static readonly Meter _meter = new("PS.FreeBookHub_Lite.PaymentService");
        private static readonly Counter<long> _duplicateCounter = _meter.CreateCounter<long>(
            "paymentservice.event.duplicates",
            unit: "{events}",
            description: "Count of duplicated OrderCreated events");

        private readonly ILogger<OrderCreatedConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly RabbitMqConfig _config;
        private readonly int _maxRetryCount;

        public OrderCreatedConsumer(
            ILogger<OrderCreatedConsumer> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<RabbitMqConfig> config)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _config = config.Value;

            _connection = CreateConnection();
            _channel = CreateChannel(_connection);
            DeclareExchanges();
            DeclareQueue();
            BindQueue();

            _maxRetryCount = _config.RetryPolicy.MaxRetryCount;

            _logger.LogInformation(LoggerMessages.OrderConsumerStarted, _config.Queues.OrderCreatedQueue);
        }

        protected override Task ExecuteAsync(CancellationToken ct)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var startTime = DateTime.UtcNow;
                Guid? orderId = null;

                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var orderCreated = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

                    if (orderCreated == null)
                        throw new JsonException("Deserialization returned null");

                    orderId = orderCreated.OrderId;
                    var messageId = ea.BasicProperties.MessageId ?? orderCreated.OrderId.ToString();

                    _logger.LogInformation(LoggerMessages.OrderMessageReceived, orderCreated.OrderId, orderCreated.UserId, orderCreated.Amount);

                    if (ExceededRetryLimit(ea, messageId))
                    {
                        return;
                    }
                        
                    _logger.LogInformation(LoggerMessages.OrderProcessingStarted, orderCreated.OrderId);

                    using var scope = _scopeFactory.CreateScope();

                    var deduplicationKey = $"processed:OrderCreatedEvent:{messageId}";
                    if (await HandleDuplicateIfExists(scope, deduplicationKey, messageId, orderCreated.OrderId, ea.DeliveryTag, ct))
                    {
                        return;
                    }

                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

                    _logger.LogInformation(LoggerMessages.PaymentProcessingStarted, orderCreated.OrderId);

                    var command = new ProcessPaymentCommand(
                        orderCreated.OrderId,
                        orderCreated.UserId,
                        orderCreated.Amount,
                        ""
                    );

                    var paymentResponse = await mediator.Send(command, ct);

                    _logger.LogInformation(LoggerMessages.PaymentProcessed, paymentResponse.Id, paymentResponse.OrderId);


                    var paymentCompleted = new PaymentCompletedEvent(
                        OrderId: paymentResponse.OrderId,
                        PaymentId: paymentResponse.Id,
                        CompletedAt: DateTime.UtcNow
                    );

                    await publisher.PublishAsync(paymentCompleted, _config.RoutingKeys.PaymentCompletedRoutingKey, ct);

                    _logger.LogInformation(LoggerMessages.PaymentEventPublished, paymentResponse.OrderId, paymentResponse.Id);

                    var elapsedMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
                    _logger.LogInformation(LoggerMessages.OrderMessageProcessed,
                        orderCreated.OrderId, elapsedMs);

                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, LoggerMessages.OrderMessageDeserializeError,
                        Encoding.UTF8.GetString(ea.Body.ToArray()));

                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, LoggerMessages.OrderProcessingError,
                        orderId?.ToString() ?? "unknown", ex.Message);

                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: _config.Queues.OrderCreatedQueue, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
            _logger.LogInformation(LoggerMessages.OrderConsumerStopped, _config.Queues.OrderCreatedQueue);
        }

        private IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config.HostName
            };
            return factory.CreateConnection();
        }

        private IModel CreateChannel(IConnection connection)
        {
            return connection.CreateModel();
        }

        private void DeclareExchanges()
        {
            _channel.ExchangeDeclare(
                _config.ExchangeName,
                ExchangeType.Topic,
                durable: true);

            _channel.ExchangeDeclare(
               _config.DeadLetter.Exchange,
               ExchangeType.Topic,
               durable: true);
        }

        private void DeclareQueue()
        {
            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", _config.DeadLetter.Exchange },
                { "x-dead-letter-routing-key", _config.DeadLetter.RoutingKey },
                { "x-message-ttl", _config.RetryPolicy.IntervalMs }
            };

            _channel.QueueDeclare(
                _config.Queues.OrderCreatedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs);
        }

        private void BindQueue()
        {
            _channel.QueueBind(
                queue: _config.Queues.OrderCreatedQueue,
                exchange: _config.ExchangeName,
                routingKey: _config.RoutingKeys.OrderCreatedRoutingKey);
        }


        private async Task<bool> HandleDuplicateIfExists(
            IServiceScope scope,
            string deduplicationKey,
            string messageId,
            Guid orderId,
            ulong deliveryTag,
            CancellationToken ct)
        {
            var dedupService = scope.ServiceProvider.GetRequiredService<IEventDeduplicationService>();

            if (await dedupService.IsDuplicateAsync(deduplicationKey, TimeSpan.FromHours(24), ct))
            {
                _duplicateCounter.Add(1, new KeyValuePair<string, object?>("event_type", "OrderCreatedEvent"));

                _logger.LogWarning(LoggerMessages.OrderMessageDuplicate, messageId, orderId);

                _channel.BasicAck(deliveryTag, false);
                return true;
            }

            return false;
        }

        private bool ExceededRetryLimit(BasicDeliverEventArgs ea, string messageId)
        {
            var retryCount = GetRetryCount(ea.BasicProperties.Headers);

            if (retryCount >= _maxRetryCount)
            {
                _logger.LogWarning(LoggerMessages.OrderMessageRetryLimitExceeded, ea.DeliveryTag, messageId);

                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                return true;
            }

            return false;
        }

        private static long GetRetryCount(IDictionary<string, object>? headers)
        {
            if (headers?.TryGetValue("x-death", out var deathHeader) != true)
                return 0;

            try
            {
                if (deathHeader is List<object> deathList &&
                    deathList.FirstOrDefault() is Dictionary<string, object> firstDeath &&
                    firstDeath.TryGetValue("count", out var countObj) &&
                    countObj is long count)
                {
                    return count;
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return 0;
        }
    }
}
