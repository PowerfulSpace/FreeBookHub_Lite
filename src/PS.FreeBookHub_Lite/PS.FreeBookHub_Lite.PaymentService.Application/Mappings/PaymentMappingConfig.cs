using Mapster;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;
using PS.FreeBookHub_Lite.PaymentService.Domain.Entities;

namespace PS.FreeBookHub_Lite.PaymentService.Application.Mappings
{
    public class PaymentMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Payment, PaymentResponse>()
                .Map(dest => dest.Status, src => src.Status.ToString());

            config.NewConfig<CreatePaymentRequest, Payment>();
        }
    }
}
