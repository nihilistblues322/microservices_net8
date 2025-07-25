using BuildingBlocks.Messaging.MassTransit;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Discount.Grpc;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

builder.Services.AddCarter();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

builder.Services
    .AddMarten(opts =>
    {
        opts.Connection(builder.Configuration.GetConnectionString("Database")!);
        opts.Schema.For<ShoppingCart>()
            .Identity(x => x.UserName)
            .Index(x => x.UserName);
    })
    .UseLightweightSessions();

builder.Services.AddStackExchangeRedisCache(opts =>
{
    opts.Configuration = builder.Configuration.GetConnectionString("Redis")!;
});

builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(opts =>
{
    opts.Address = new Uri(builder.Configuration.GetConnectionString("DiscountGrpcService")!);
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddMessageBroker(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);


var app = builder.Build();

app.MapCarter();
app.UseExceptionHandler(opts => { });
app.UseHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    }
);

app.Run();