using Anko.WarehouseService.Core.Repositories;

namespace Anko.WarehouseService.Api.Endpoints.Items;

public static class DeleteItemEndpoint
{
    public const string Name = "DeleteItem";

    public static IEndpointRouteBuilder MapDeleteItem(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/items/{id}", async (Guid id, IItemsRepository repository) =>
        {
            var existingItem = await repository.GetItemByIdAsync(id.ToString());
            if (existingItem == null)
            {
                return Results.NotFound();
            }

            await repository.DeleteItemAsync(id.ToString());
            return Results.NoContent();
        });
        return app;
    }
}