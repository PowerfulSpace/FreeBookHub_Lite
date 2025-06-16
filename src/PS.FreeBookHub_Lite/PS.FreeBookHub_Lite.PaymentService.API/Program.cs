using PS.FreeBookHub_Lite.PaymentService.API;
using PS.FreeBookHub_Lite.PaymentService.API.Logging;
using PS.FreeBookHub_Lite.PaymentService.API.Middleware;
using PS.FreeBookHub_Lite.PaymentService.Application;
using PS.FreeBookHub_Lite.PaymentService.Infrastructure;
using Serilog;

SerilogBootstrapper.ConfigureSerilog();

try
{
    Log.Information("Starting up [PaymentService]...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

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
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly [PaymentService]");
}
finally
{
    Log.Information("Shut down complete.[PaymentService]");
    Log.CloseAndFlush();
}