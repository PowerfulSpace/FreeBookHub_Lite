using DotNetEnv;
using PS.PaymentService.API;
using PS.PaymentService.API.Logging;
using PS.PaymentService.API.Middleware;
using PS.PaymentService.Application;
using PS.PaymentService.Infrastructure;
using PS.PaymentService.Common.Extensions.DependencyInjection;
using PS.PaymentService.Common.Extensions.Hosting;
using PS.PaymentService.Infrastructure.StartupTasks;
using Serilog;

SerilogBootstrapper.ConfigureSerilog();

try
{
    Log.Information("Starting up [PaymentService]...");

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
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeBookHub Payment API v1");
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
    Log.Fatal(ex, "Application terminated unexpectedly [PaymentService]");
}
finally
{
    Log.Information("Shut down complete.[PaymentService]");
    Log.CloseAndFlush();
}