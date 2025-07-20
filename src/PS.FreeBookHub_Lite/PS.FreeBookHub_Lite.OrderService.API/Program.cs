using PS.FreeBookHub_Lite.OrderService.API;
using PS.FreeBookHub_Lite.OrderService.API.Logging;
using PS.FreeBookHub_Lite.OrderService.API.Middleware;
using PS.FreeBookHub_Lite.OrderService.Application;
using PS.FreeBookHub_Lite.OrderService.Infrastructure;
using Serilog;


SerilogBootstrapper.ConfigureSerilog();

try
{
    Log.Information("Starting up [OrderService]...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services
        .AddPresentation(builder.Configuration)
        .AddInfrastructure(builder.Configuration)
        .AddApplication(builder.Configuration);

    var app = builder.Build();
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeBookHub Order API v1");
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
    Log.Fatal(ex, "Application terminated unexpectedly [OrderService]");
}
finally
{
    Log.Information("Shut down complete.[OrderService]");
    Log.CloseAndFlush();
}