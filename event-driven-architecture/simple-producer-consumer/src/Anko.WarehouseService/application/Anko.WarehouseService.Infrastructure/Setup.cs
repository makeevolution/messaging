// using MongoDB.Driver;
//
// namespace Anko.WarehouseService.Infrastructure
// {
//     public static class Setup
//     {
//         public static IServiceCollection AddRecipeInfrastructure(this IServiceCollection services,
//             IConfiguration configuration)
//         {
//             var client = new MongoClient(configuration["DatabaseConnection"]);
//
//             services.AddSingleton(client);
//         }
//     }
// }