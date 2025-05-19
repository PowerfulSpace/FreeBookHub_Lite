namespace PS.FreeBookHub_Lite.PaymentService.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(options => options.EnableAnnotations());
            return services;
        }
    }
}
