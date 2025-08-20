using DotNetEnv;
using PS.FreeBookHub_Lite.CatalogService.API;
using PS.FreeBookHub_Lite.CatalogService.API.Logging;
using PS.FreeBookHub_Lite.CatalogService.API.Middleware;
using PS.FreeBookHub_Lite.CatalogService.Application;
using PS.FreeBookHub_Lite.CatalogService.Infrastructure;
using Serilog;


SerilogBootstrapper.ConfigureSerilog();

try
{
    Log.Information("Starting up [CatalogService]...");

    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
    {
        if (File.Exists(".env.development"))
        {
            Env.Load(".env.development");
        }
        else
        {
            Log.Warning(".env.development not found. Using default configuration.");
        }
    }

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
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeBookHub Catalog API v1");
                options.RoutePrefix = string.Empty;
            });
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (!app.Environment.IsEnvironment("Docker"))
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly [CatalogService]");
}
finally
{
    Log.Information("Shut down complete.[CatalogService]");
    Log.CloseAndFlush();
}