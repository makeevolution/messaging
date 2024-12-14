// Entry point to the whole app
using EDA.Producer;
using EDA.Producer.Adapters;
using EDA.Producer.Core;
using Microsoft.EntityFrameworkCore;

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

// Next, we add services to the DI container (for dependency injection resolve)
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Configuration gets its values from appsettings.json or appsettings.Development.json
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddDatabase(builder.Configuration);

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
// And, use using to dispose of the service
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

static async Task<Order> HandleCreateOrder(CreateOrderRequest request, IOrders orders, IEventPublisher events)
{
    var order = await orders.New(request.CustomerId);

    await events.Publish(new OrderCreatedEvent()
    {
        OrderId = order.OrderId
    });

    return order;
}

static async Task<string> HandleHealthCheck(OrdersDbContext context)
{
    await context.Database.MigrateAsync();
    return "OK";
}
