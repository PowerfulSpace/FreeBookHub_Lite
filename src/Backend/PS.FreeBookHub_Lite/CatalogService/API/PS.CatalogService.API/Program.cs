using DotNetEnv;
using PS.CatalogService.API;
using PS.CatalogService.API.Logging;
using PS.CatalogService.API.Middleware;
using PS.CatalogService.Application;
using PS.CatalogService.Infrastructure;
using PS.CatalogService.Common.Extensions.DependencyInjection;
using PS.CatalogService.Common.Extensions.Hosting;
using PS.CatalogService.Infrastructure.StartupTasks;
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

    builder.Services.AddStartupTask<DatabaseMigrationStartupTask>();

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

        app.RunStartupTasks();

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