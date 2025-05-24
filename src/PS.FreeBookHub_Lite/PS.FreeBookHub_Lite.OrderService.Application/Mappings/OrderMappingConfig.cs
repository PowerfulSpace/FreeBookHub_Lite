using Mapster;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;

namespace PS.FreeBookHub_Lite.OrderService.Application.Mappings
{
    public class OrderMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateOrderRequest, OrderResponse>();
            config.NewConfig<CreateOrderItemRequest, OrderItemDto>();
        }
    }
}
