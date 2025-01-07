namespace PlantBasedPizza.OrderManager.Core.AddItemToOrder
{
    public class AddItemToOrderCommand
    {
        public string CustomerIdentifier { get; set; } = "";
        
        public string OrderIdentifier { get; init; } = "";
        
        public string RecipeIdentifier { get; init; } = "";
        
        public int Quantity { get; init; }
    }
}