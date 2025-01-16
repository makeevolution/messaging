// Entry point to the producer/publisher project
// See README.md for more info

using EDA.Producer;
using EDA.Producer.Adapters;
using EDA.Producer.Core;
using Microsoft.EntityFrameworkCore;
using shared;

var builder = WebApplication.CreateBuilder(args);

// What's going on under the hood?
// When you do dotnet run, ASP.NET will auto launch with the settings specified in the first entry of "profiles"
// in Properties/launchSettings.json
// You can specify which profile using dotnet run --project pathToYourProj --launch-profile desiredProfile
// If you run using C# VSCode devkit though, it will always run https by default

// Then we will load appsettings.
// Typically each "profile" contains the ASPNETCORE_ENVIRONMENT env var.
// builder above will automatically first appsettings.json, and, appsettings.{ASPNETCORE_ENVIRONMENT}.json
// builder.Configuration will contain all the contents of these appsettings, plus more internal stuff!

// Next, we add services to the DI container
// Services is an IServiceCollection which will contain services that can be resolved by the DI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Where does ConfigureMessaging and ConfigureDatabase services below come from???
// They come from implementing extension methods feature of .NET
// In your project code, where ever you define a static class and a static method that accepts an interface as 'this' argument e.g.:
// public static class someStaticClass
// {
//     public static IServiceCollection someStaticMethod(this IServiceCollection services, IConfiguration configuration)
//     {
//         services.AddDbContext<OrdersDbContext>(opt =>
//             opt.UseNpgsql(
//                 configuration.GetConnectionString("OrdersContext"),
//                 o => o
//                     .SetPostgresVersion(17, 0)));

//         services.AddScoped<IOrders, PostgresOrders>();

//         return services;
//     }
// }
// Then you have implemented the extension method feature. The IServiceCollection will now contain someStaticMethod.
// For the two below, they are implemented in ServiceExtensions.cs
// Also, the builder.Configuration below gets its values from appsettings.json or appsettings.Development.json
builder.Services.ConfigureMessaging(builder.Configuration);
builder.Services.ConfigureDatabase(builder.Configuration);

// The publishing itself will happen as a BackgroundService; see the code in OutboxWorker
// Here we register the background service itself so it will actually run!
builder.Services.AddHostedService<OutboxWorker>();

builder.Services.AddSharedInfrastructure(builder.Configuration, "EDA.producer");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.Services is the DI container of the ASP.NET Core app
var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
// Services can have different lifetimes: singleton, transient and scoped.
// Use CreateScope to obtain a dbContext service instance that is always the same instance everytime
// dbContext is called.
// And, use using to dispose of the service.
using (var scope = serviceScopeFactory.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapGet("/health", HandleHealthCheck)
.WithName("HealthCheck")
.WithOpenApi();

app.MapPost("/orders", HandleCreateOrder)
    .WithName("CreateOrder")
    .WithOpenApi();

app.Run();

// The arguments below that are interfaces, are injected by the DI framework
static async Task<Order> HandleCreateOrder(CreateOrderRequest request, IOrders orders, IEventPublisher events)
{
    // Create new order for customer in db
    var order = await orders.New(request.CustomerId);
    return order;
}

static async Task<string> HandleHealthCheck(OrdersDbContext context)
{
    await context.Database.MigrateAsync();
    return "OK";
}
