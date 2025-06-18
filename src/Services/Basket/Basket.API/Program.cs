var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

builder.Services.AddCarter();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services
    .AddMarten(opts =>
    {
        opts.Connection(builder.Configuration.GetConnectionString("Database")!);
        opts.Schema.For<ShoppingCart>().Index(x => x.UserName);
    })
    .UseLightweightSessions();
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

app.MapCarter();
app.UseExceptionHandler(opts => { });

app.Run();