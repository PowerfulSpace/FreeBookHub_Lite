using Mapster;
using PS.FreeBookHub_Lite.CartService.Application.CQRS.Commands.AddItem;
using PS.FreeBookHub_Lite.CartService.Application.CQRS.Commands.Checkout;
using PS.FreeBookHub_Lite.CartService.Application.CQRS.Commands.UpdateItemQuantity;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Cart;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Order;
using PS.FreeBookHub_Lite.CartService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CartService.Application.Mappings
{
    public class CartMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Cart, CartResponse>();
            config.NewConfig<CartItem, CartItemDto>();

            config.NewConfig<AddItemRequest, AddItemCommand>()
                .Map(dest => dest.UserId, src => MapContext.Current!.Parameters["UserId"]);

            config.NewConfig<UpdateItemQuantityRequest, UpdateItemQuantityCommand>()
               .Map(dest => dest.UserId, src => MapContext.Current!.Parameters["UserId"]);

            config.NewConfig<CheckoutRequest, CheckoutCommand>()
              .Map(dest => dest.UserId, src => MapContext.Current!.Parameters["UserId"]);

        }
    }
}
