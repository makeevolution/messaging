// This is our consumer service
// For more info on how things work first study Producer's program.cs

using EDA.Consumer;
using EDA.Consumer.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessaging(builder.Configuration);
// Also need to add service to consume messages (i.e. OrderCreatedEventWorker)
// It will run as a BackgroundService
builder.Services.AddHostedService<OrderCreatedEventWorker>();

var app = builder.Build();
app.Run();
