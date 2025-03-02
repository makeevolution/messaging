using Anko.Shared;
using Anko.Shared.Authentication;
using Anko.WarehouseService;
using Anko.WarehouseService.Api.Endpoints;
using Anko.WarehouseService.Api.Endpoints.Items;
using Anko.WarehouseService.Core.Repositories;
using Anko.WarehouseService.Infrastructure;
using Anko.WarehouseService.Infrastructure.Repositories;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddSingleton<IItemsRepository, ItemsRepository>();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

// Add warehouse-specific infrastructure, and also shared infrastructure
builder.Services.AddWarehouseInfrastructure(builder.Configuration);
builder.Host.AddSharedInfrastructure(builder.Configuration, ApplicationDefaults.ServiceName);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Add CORS policy
app.UseCors(CorsSettings.ALLOW_ALL_POLICY_NAME);
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapItemsEndpoints();
});
var itemsRepo = app.Services.GetRequiredService<IItemsRepository>();
await itemsRepo.SeedItemsAsync();

app.UseSwagger();
app.UseSwaggerUI();
app.Run();
