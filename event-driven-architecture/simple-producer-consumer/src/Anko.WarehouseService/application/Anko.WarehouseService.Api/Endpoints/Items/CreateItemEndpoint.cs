
using Anko.WarehouseService.Core.Entities;
using Anko.WarehouseService.Core.Repositories;

namespace Anko.WarehouseService.Api.Endpoints.Items;

public static class CreateItemEndpoint
{
    public const string Name = "CreateItem";

    public static IEndpointRouteBuilder MapCreateItem(this IEndpointRouteBuilder app)
    {
        app.MapPost("/items", async (Item item, IItemsRepository repository) =>
        {
            await repository.AddItemAsync(item);
            return Results.Created($"/items/{item.ItemIdentifier}", item);
        });
        return app;
    }
}