// This is our consumer service
// For more info on how things work first study Producer's program.cs

using EDA.Consumer;
using EDA.Consumer.Core;
using EDA.Consumer.Adapters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessaging(builder.Configuration);
// Also need to add service to consume messages (i.e. OrderCreatedEventWorker)
// It will run as a BackgroundService
builder.Services.AddHostedService<OrderCreatedEventWorker>();
// If you see OrderCreatedEventWorker above, you will see that it needs an OrderCreatedEventHandler!
// To register that specific implementation, since we are not using ASP.NET here (i.e. no HTTP requests),
// can only either register the service as a singleton or transient. Just choose singleton to save
// performance cost of recreating a new handler everytime we call.
builder.Services.AddScoped<OrderCreatedEventHandler>();

var app = builder.Build();
app.Run();
