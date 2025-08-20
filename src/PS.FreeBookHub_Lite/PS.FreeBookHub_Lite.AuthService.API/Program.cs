using DotNetEnv;
using PS.FreeBookHub_Lite.AuthService.API;
using PS.FreeBookHub_Lite.AuthService.API.Logging;
using PS.FreeBookHub_Lite.AuthService.API.Middleware;
using PS.FreeBookHub_Lite.AuthService.Application;
using PS.FreeBookHub_Lite.AuthService.Infrastructure;
using Serilog;



SerilogBootstrapper.ConfigureSerilog();

try
{
    Log.Information("Starting up [AuthService]...");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services
        .AddPresentation(builder.Configuration)
        .AddInfrastructure(builder.Configuration)
        .AddApplication();

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

    var app = builder.Build();
    {

        if (app.Environment.IsDevelopment())
        {
            //app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeBookHub Auth API v1");
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
    Log.Fatal(ex, "Application terminated unexpectedly [AuthService]");
}
finally
{
    Log.Information("Shut down complete.[AuthService]");
    Log.CloseAndFlush();
}

public partial class Program { }