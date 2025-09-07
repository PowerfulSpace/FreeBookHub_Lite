using MediatR;
using PS.OrderService.Application.DTOs;

namespace PS.OrderService.Application.CQRS.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<OrderResponse>
    {
        public Guid UserId { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}
