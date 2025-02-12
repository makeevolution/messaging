// This is our consumer service
// For more info on how things work first study Producer's program.cs

// In summary on how to setup a pub sub RabbitMQ client:

// 1.
// Create a connection instance, where the connection is a field
// i.e. any instance of the connection above will use the same connection
// (to prevent our stupid VFM2 TooManyConnections issues!!!)
// This is set in AddMessaging function below.

// 2.
// Our background worker will use the instance to create a configured channel
// A configured channel consists of:
// - A channel which has queues and exchanges bound using routing keys
// - A consumer that subscribes to messages to the queues above


using Anko.PaymentsService;
using Anko.PaymentsService.Core;
using Serilog;
using Anko.Shared;

var builder = WebApplication.CreateBuilder(args);
// Add JSON builder to add env.Environment to add appsettings.production

builder.Services.AddMessaging(builder.Configuration);
// Also need to add service to consume messages (i.e. orderSubmittedEventWorker)
// It will run as a BackgroundService
builder.Services.AddHostedService<orderSubmittedEventWorker>();
// If you see orderSubmittedEventWorker above, you will see that it needs an orderSubmittedEventHandler!
// To register that specific implementation, since we are not using ASP.NET here (i.e. no HTTP requests),
// can only either register the service as a singleton or transient. Just choose singleton to save
// performance cost of recreating a new handler everytime we call.
builder.Services.AddTransient<orderSubmittedEventHandler>();

// Add OTEL
builder.Host.AddSharedInfrastructure(builder.Configuration, ApplicationDefaults.ServiceName);

var app = builder.Build();
app.UseSerilogRequestLogging();
await app.RunAsync();

