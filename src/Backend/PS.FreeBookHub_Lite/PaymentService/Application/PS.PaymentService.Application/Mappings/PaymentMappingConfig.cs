using Mapster;
using PS.PaymentService.Application.CQRS.Commands.ProcessPayment;
using PS.PaymentService.Application.DTOs;
using PS.PaymentService.Domain.Entities;

namespace PS.PaymentService.Application.Mappings
{
    public class PaymentMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Payment, PaymentResponse>()
                .Map(dest => dest.Status, src => src.Status.ToString());

            config.NewConfig<CreatePaymentRequest, Payment>();


            config.NewConfig<CreatePaymentRequest, ProcessPaymentCommand>();
        }
    }
}
