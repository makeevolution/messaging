using Anko.WarehouseService.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Anko.WarehouseService.Core.Repositories
{
    public interface IItemsRepository
    {
        Task<Item> GetItemByIdAsync(string itemId);
        Task<IEnumerable<Item>> GetAllItemsAsync();
        Task AddItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(string itemId);

        // Seed items on startup, just for demo purposes
        Task SeedItemsAsync();
    }
}
