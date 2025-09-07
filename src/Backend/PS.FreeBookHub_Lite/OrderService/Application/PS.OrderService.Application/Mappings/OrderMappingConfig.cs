using Mapster;
using PS.OrderService.Application.CQRS.Commands.CreateOrder;
using PS.OrderService.Application.DTOs;
using PS.OrderService.Domain.Entities;

namespace PS.OrderService.Application.Mappings
{
    public class OrderMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            //config.NewConfig<CreateOrderRequest, OrderResponse>();

            //config.NewConfig<CreateOrderItemRequest, OrderItemDto>();


            config.NewConfig<CreateOrderRequest, CreateOrderCommand>();

            config.NewConfig<Order, OrderResponse>();
        }
    }
}
