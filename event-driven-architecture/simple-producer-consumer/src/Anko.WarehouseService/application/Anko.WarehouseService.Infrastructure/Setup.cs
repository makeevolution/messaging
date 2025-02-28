 using Anko.WarehouseService.Core.Entities;
 using MongoDB.Bson;
 using MongoDB.Bson.Serialization;
 using MongoDB.Bson.Serialization.Serializers;
 using MongoDB.Driver;
 using Microsoft.Extensions.Configuration;
 using Microsoft.Extensions.DependencyInjection;

 namespace Anko.WarehouseService.Infrastructure
 {
     public static class Setup
     {
         public static IServiceCollection AddWarehouseInfrastructure(this IServiceCollection services,
             IConfiguration configuration)
         {
             services.AddSingleton(new MongoClient(configuration["DatabaseConnection"]));
             BsonClassMap.RegisterClassMap<Item>(map =>
             {
                 map.AutoMap();
                 map.SetIgnoreExtraElements(true);
                 map.SetIgnoreExtraElementsIsInherited(true);
             });
             // Set GuidSerializer to Standard representation
             BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
             return services;
         }
     }
 }