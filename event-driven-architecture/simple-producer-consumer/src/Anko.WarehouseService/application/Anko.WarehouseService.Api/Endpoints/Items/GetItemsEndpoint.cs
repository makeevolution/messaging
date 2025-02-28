using Anko.WarehouseService.Core.Repositories;

namespace Anko.WarehouseService.Api.Endpoints.Items;

public static class GetItemsEndpoint
{
    public const string Name = "GetItems";

    public static IEndpointRouteBuilder MapGetItems(this IEndpointRouteBuilder app)
    {
        app.MapGet("/items", async (IItemsRepository repository) =>
        {
            var items = await repository.GetAllItemsAsync();
            return Results.Ok(items);
        });
        return app;
    }
}