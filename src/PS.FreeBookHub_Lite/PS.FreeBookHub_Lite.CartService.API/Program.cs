using PS.FreeBookHub_Lite.CartService.API;
using PS.FreeBookHub_Lite.CartService.API.Logging;
using PS.FreeBookHub_Lite.CartService.API.Middleware;
using PS.FreeBookHub_Lite.CartService.Application;
using PS.FreeBookHub_Lite.CartService.Infrastructure;
using Serilog;



SerilogBootstrapper.ConfigureSerilog();

try
{
    Log.Information("Starting up [CartService]...");

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
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeBookHub Cart API v1");
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
    Log.Fatal(ex, "Application terminated unexpectedly [CartService]");
}
finally
{
    Log.Information("Shut down complete.[CartService]");
    Log.CloseAndFlush();
}