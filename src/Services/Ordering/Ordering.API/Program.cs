var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseApiServices();
if (app.Environment.IsDevelopment())
{
    await app.InitializeDatabaseAsync();
}

app.Run();