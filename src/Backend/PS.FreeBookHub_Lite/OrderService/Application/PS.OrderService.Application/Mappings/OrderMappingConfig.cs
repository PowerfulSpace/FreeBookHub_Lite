using Mapster;
using PS.FreeBookHub_Lite.OrderService.Application.CQRS.Commands.CreateOrder;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using PS.FreeBookHub_Lite.OrderService.Domain.Entities;

namespace PS.FreeBookHub_Lite.OrderService.Application.Mappings
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
