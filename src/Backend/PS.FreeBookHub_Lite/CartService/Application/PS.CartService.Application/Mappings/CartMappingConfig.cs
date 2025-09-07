using Mapster;
using PS.CartService.Application.CQRS.Commands.AddItem;
using PS.CartService.Application.CQRS.Commands.Checkout;
using PS.CartService.Application.CQRS.Commands.UpdateItemQuantity;
using PS.CartService.Application.DTOs.Cart;
using PS.CartService.Application.DTOs.Order;
using PS.CartService.Domain.Entities;

namespace PS.CartService.Application.Mappings
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
