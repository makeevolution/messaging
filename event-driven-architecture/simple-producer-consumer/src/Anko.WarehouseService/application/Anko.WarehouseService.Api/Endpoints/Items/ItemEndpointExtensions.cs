namespace Anko.WarehouseService.Api.Endpoints.Items;

public static class ItemEndpointExtensions
{
    public static IEndpointRouteBuilder MapItemsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetItems();
        app.MapCreateItem();
        app.MapGetItemById();
        app.MapUpdateItem();
        app.MapDeleteItem();
        return app;
    }
}