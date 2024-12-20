namespace PlantBasedPizza.Deliver.Infrastructure;

public static class InfrastructureConstants
{
    public static string TableName => Environment.GetEnvironmentVariable("TABLE_NAME") ?? "plant-based-pizza";
}