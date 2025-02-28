using Anko.WarehouseService.Core.Repositories;

namespace Anko.WarehouseService.Api.Endpoints.Items;

public static class GetItemByIdEndpoint
{
    public const string Name = "GetItemById";

    public static IEndpointRouteBuilder MapGetItemById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/items/{id}", async (Guid id, IItemsRepository repository) =>
        {
            var item = await repository.GetItemByIdAsync(id.ToString());
            return item != null ? Results.Ok(item) : Results.NotFound();
        });
        return app;
    }
}