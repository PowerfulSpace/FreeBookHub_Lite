using Serilog;

namespace PS.FreeBookHub_Lite.CatalogService.API.Logging
{
    public static class SerilogBootstrapper
    {
        public static void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build())
                .Enrich.FromLogContext()
                .CreateLogger();
        }
    }
}
