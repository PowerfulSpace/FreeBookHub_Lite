using DotNetEnv;
using PS.OrderService.API;
using PS.OrderService.API.Logging;
using PS.OrderService.API.Middleware;
using PS.OrderService.Application;
using PS.OrderService.Infrastructure;
using PS.OrderService.Common.Extensions.DependencyInjection;
using PS.OrderService.Common.Extensions.Hosting;
using Serilog;
using PS.OrderService.Infrastructure.StartupTasks;


SerilogBootstrapper.ConfigureSerilog();

try
{
    Log.Information("Starting up [OrderService]...");

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
        .AddApplication(builder.Configuration);

    builder.Services.AddStartupTask<DatabaseMigrationStartupTask>();

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
    Log.Fatal(ex, "Application terminated unexpectedly [OrderService]");
}
finally
{
    Log.Information("Shut down complete.[OrderService]");
    Log.CloseAndFlush();
}