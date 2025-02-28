using Anko.WarehouseService.Api.Endpoints;
using Anko.WarehouseService.Core.Entities;
using Anko.WarehouseService.Core.Repositories;
using Anko.WarehouseService.Infrastructure.Repositories;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddSingleton<IItemsRepository, ItemsRepository>();
builder.Services.AddSingleton(new MongoClient(builder.Configuration["DatabaseConnection"]));
BsonClassMap.RegisterClassMap<Item>(map =>
{
    map.AutoMap();
    map.SetIgnoreExtraElements(true);
    map.SetIgnoreExtraElementsIsInherited(true);
});
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapItemEndpoints();
});
var itemsRepo = app.Services.GetRequiredService<IItemsRepository>();
await itemsRepo.SeedItemsAsync();
app.Run();
