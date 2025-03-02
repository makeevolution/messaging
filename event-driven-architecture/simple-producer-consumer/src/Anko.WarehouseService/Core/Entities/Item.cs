using System.Text.Json.Serialization;

namespace Anko.WarehouseService.Core.Entities;

public enum ItemCategory
{
    Smartphone,
    Laptop,
    Tablet,
    PC,
    Monitor,
}

public class Item
{
    [JsonPropertyName("name")]
    public string Name { get; private set; }
    [JsonPropertyName("itemIdentifier")]
    public Guid ItemIdentifier { get; private set; }
    [JsonPropertyName("price")]
    public decimal Price { get; private set; }
    [JsonPropertyName("category")]
    public ItemCategory Category { get; private set; }

    [JsonConstructor]
    private Item()
    {
        ItemIdentifier = Guid.Empty;
        Name = "";
    }

    public Item(ItemCategory category, string itemIdentifier, string name, decimal price)
    {
        Category = category;
        ItemIdentifier = Guid.Parse(itemIdentifier);
        Name = name;
        Price = price;
    }
}