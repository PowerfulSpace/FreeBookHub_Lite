using PS.FreeBookHub_Lite.CartService.API;
using PS.FreeBookHub_Lite.CartService.Application;
using PS.FreeBookHub_Lite.CartService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation()
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

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}


