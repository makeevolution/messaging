// Consumer services

using EDA.Consumer;
using EDA.Consumer.Adapters;
using EDA.Consumer.Core;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddMessaging(builder.Configuration);

app.Run();
