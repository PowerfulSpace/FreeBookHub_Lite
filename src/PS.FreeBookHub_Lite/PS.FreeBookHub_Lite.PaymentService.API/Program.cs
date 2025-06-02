using PS.FreeBookHub_Lite.PaymentService.API;
using PS.FreeBookHub_Lite.PaymentService.API.Middleware;
using PS.FreeBookHub_Lite.PaymentService.Application;
using PS.FreeBookHub_Lite.PaymentService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApplication();


var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeBookHub Payment API v1");
            options.RoutePrefix = string.Empty;
        });
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}