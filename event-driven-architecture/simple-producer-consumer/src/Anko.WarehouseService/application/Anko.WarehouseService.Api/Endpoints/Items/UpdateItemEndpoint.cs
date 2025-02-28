using Anko.WarehouseService.Core.Entities;
using Anko.WarehouseService.Core.Repositories;

namespace Anko.WarehouseService.Api.Endpoints.Items;

public static class UpdateItemEndpoint
{
    public const string Name = "UpdateItem";

    public static IEndpointRouteBuilder MapUpdateItem(this IEndpointRouteBuilder app)
    {
        app.MapPut("/items/{id}", async (Guid id, Item updatedItem, IItemsRepository repository) =>
        {
            var existingItem = await repository.GetItemByIdAsync(id.ToString());
            if (existingItem == null)
            {
                return Results.NotFound();
            }

            updatedItem = new Item(updatedItem.Category, id.ToString(), updatedItem.Name, updatedItem.Price);
            await repository.UpdateItemAsync(updatedItem);
            return Results.NoContent();
        });
        return app;
    }
}