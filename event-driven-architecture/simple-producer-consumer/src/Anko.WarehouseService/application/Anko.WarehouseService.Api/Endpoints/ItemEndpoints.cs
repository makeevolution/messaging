using Anko.WarehouseService.Core.Entities;
using Anko.WarehouseService.Core.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace Anko.WarehouseService.Api.Endpoints
{
    public static class ItemEndpoints
    {
        public static void MapItemEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/items", async (IItemsRepository repository) =>
            {
                var items = await repository.GetAllItemsAsync();
                return Results.Ok(items);
            });

            routes.MapGet("/items/{id}", async (Guid id, IItemsRepository repository) =>
            {
                var item = await repository.GetItemByIdAsync(id.ToString());
                return item != null ? Results.Ok(item) : Results.NotFound();
            });

            routes.MapPost("/items", async (Item item, IItemsRepository repository) =>
            {
                await repository.AddItemAsync(item);
                return Results.Created($"/items/{item.ItemIdentifier}", item);
            });

            routes.MapPut("/items/{id}", async (Guid id, Item updatedItem, IItemsRepository repository) =>
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

            routes.MapDelete("/items/{id}", async (Guid id, IItemsRepository repository) =>
            {
                var existingItem = await repository.GetItemByIdAsync(id.ToString());
                if (existingItem == null)
                {
                    return Results.NotFound();
                }

                await repository.DeleteItemAsync(id.ToString());
                return Results.NoContent();
            });
        }
    }
}
