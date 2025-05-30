using PS.FreeBookHub_Lite.AuthService.API;
using PS.FreeBookHub_Lite.AuthService.API.Middleware;
using PS.FreeBookHub_Lite.AuthService.Application;
using PS.FreeBookHub_Lite.AuthService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeBookHub Auth API v1");
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
