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

// Add shared infrastructure
builder.Services.AddWarehouseInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


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
