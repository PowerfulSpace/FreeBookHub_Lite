using Mapster;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Cart;
using PS.FreeBookHub_Lite.CartService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CartService.Application.Mappings
{
    public class CartMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Cart, CartResponse>();
            config.NewConfig<CartItem, CartItemDto>();
        }
    }
}
