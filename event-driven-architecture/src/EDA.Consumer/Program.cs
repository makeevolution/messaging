// This is our consumer service

// The consumer will run as a BackgroundService

using EDA.Consumer;
using EDA.Consumer.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessaging(builder.Configuration);

var app = builder.Build();
app.Run();
