using Mapster;
using PS.FreeBookHub_Lite.CartService.Application.DTOs;
using PS.FreeBookHub_Lite.CartService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CartService.Application.Mappings
{
    public class CartMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Cart, CartDto>();
            config.NewConfig<CartItem, CartItemDto>();
        }
    }
}
