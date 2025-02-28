using Anko.WarehouseService.Core.Entities;
using Anko.WarehouseService.Core.Repositories;
using MongoDB.Driver;

namespace Anko.WarehouseService.Infrastructure.Repositories
{
    public class ItemsRepository : IItemsRepository
    {
        private readonly IMongoCollection<Item> _itemsCollection;

        public ItemsRepository(MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("WarehouseDb");
            _itemsCollection = database.GetCollection<Item>("Items");
        }

        public async Task<Item> GetItemByIdAsync(string itemId)
        {
            var filter = Builders<Item>.Filter.Eq("ItemIdentifier", itemId);
            return await _itemsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await _itemsCollection.Find(_ => true).ToListAsync();
        }

        public async Task AddItemAsync(Item item)
        {
            await _itemsCollection.InsertOneAsync(item);
        }

        public async Task UpdateItemAsync(Item item)
        {
            var filter = Builders<Item>.Filter.Eq(existingItem => existingItem.ItemIdentifier, item.ItemIdentifier);
            await _itemsCollection.ReplaceOneAsync(filter, item);
        }

        public async Task DeleteItemAsync(string itemId)
        {
            var filter = Builders<Item>.Filter.Eq("ItemIdentifier", itemId);
            await _itemsCollection.DeleteOneAsync(filter);
        }

        public async Task SeedItemsAsync()
        {
            var items = new List<Item>
            {
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "iPhone", (decimal)999.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Samsung Galaxy", (decimal)899.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Google Pixel", (decimal)799.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "OnePlus", (decimal)699.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Sony Xperia", (decimal)749.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Nokia", (decimal)599.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Huawei", (decimal)649.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Xiaomi", (decimal)549.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Oppo", (decimal)499.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Vivo", (decimal)449.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "MacBook Pro", (decimal)1299.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Dell XPS", (decimal)1199.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "HP Spectre", (decimal)1099.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Lenovo ThinkPad", (decimal)999.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Asus ZenBook", (decimal)899.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Acer Swift", (decimal)799.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Microsoft Surface", (decimal)1299.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Razer Blade", (decimal)1499.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "MSI Prestige", (decimal)1399.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "LG Gram", (decimal)1199.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "iPad", (decimal)499.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Samsung Galaxy Tab", (decimal)399.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Amazon Fire", (decimal)199.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Microsoft Surface Go", (decimal)599.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Lenovo Tab", (decimal)299.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Huawei MediaPad", (decimal)349.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Asus ZenPad", (decimal)249.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Acer Iconia", (decimal)199.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Google Nexus", (decimal)399.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Sony Xperia Tablet", (decimal)449.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Dell Inspiron", (decimal)699.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "HP Pavilion", (decimal)799.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Lenovo ThinkCentre", (decimal)899.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Acer Aspire", (decimal)599.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Asus ROG", (decimal)1299.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "MSI Trident", (decimal)1399.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "CyberPowerPC", (decimal)1499.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "iBUYPOWER", (decimal)1599.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Alienware", (decimal)1699.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Corsair One", (decimal)1799.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Dell UltraSharp", (decimal)299.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Samsung Curved", (decimal)399.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "LG UltraWide", (decimal)499.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Asus ROG Swift", (decimal)599.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Acer Predator", (decimal)699.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "BenQ Zowie", (decimal)249.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "HP Omen", (decimal)349.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "MSI Optix", (decimal)449.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "ViewSonic Elite", (decimal)549.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Gigabyte Aorus", (decimal)649.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Motorola", (decimal)399.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Realme", (decimal)299.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "ZTE", (decimal)199.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Meizu", (decimal)249.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Alcatel", (decimal)149.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Gigabyte Aero", (decimal)1599.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Alienware m15", (decimal)1799.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Samsung Notebook", (decimal)999.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Huawei MateBook", (decimal)899.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Google Pixelbook", (decimal)1299.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Nokia T20", (decimal)199.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "TCL Tab", 149.99m),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Vankyo MatrixPad", (decimal)99.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Dragon Touch", (decimal)129.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Fusion5", (decimal)179.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "HP EliteDesk", (decimal)999.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Lenovo Legion", (decimal)1199.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Asus VivoPC", (decimal)799.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Dell XPS Tower", (decimal)1099.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "MSI Infinite", (decimal)1299.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Philips Brilliance", (decimal)299.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "AOC Agon", (decimal)399.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Samsung Odyssey", (decimal)499.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "LG UltraGear", (decimal)599.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Dell Alienware", (decimal)699.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Fairphone", (decimal)499.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Cat", (decimal)599.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "BlackBerry", (decimal)699.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Palm", (decimal)199.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Essential", (decimal)299.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Toshiba Dynabook", (decimal)899.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Panasonic Toughbook", (decimal)1999.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Chuwi HeroBook", (decimal)299.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "VAIO", (decimal)1499.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "System76", (decimal)1299.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Chuwi Hi10", (decimal)199.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Teclast M40", (decimal)149.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "ALLDOCUBE", (decimal)99.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "BOOX Note", (decimal)399.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Onyx Boox", (decimal)499.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Zotac ZBOX", (decimal)599.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Intel NUC", (decimal)699.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "ASRock DeskMini", (decimal)799.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Gigabyte Brix", (decimal)899.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Shuttle XPC", (decimal)999.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Eizo FlexScan", (decimal)299.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "NEC MultiSync", (decimal)399.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Sharp PN", (decimal)499.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Panasonic TH", (decimal)599.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Sony Bravia", (decimal)699.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Infinix", (decimal)199.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Tecno", (decimal)149.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Itel", (decimal)99.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Micromax", (decimal)129.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Lava", (decimal)79.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Clevo", (decimal)1099.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Eurocom", (decimal)1299.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Origin PC", (decimal)1499.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Eluktronics", (decimal)1699.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Sager", (decimal)1899.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Voyo i8", (decimal)199.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Jumper EZpad", (decimal)149.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Cube iPlay", (decimal)99.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Pipo W", (decimal)129.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Onda V", (decimal)179.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Beelink GT", (decimal)599.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "MINIX NEO", (decimal)699.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Vorke V", (decimal)799.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Terryza", (decimal)899.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "ACEPC", (decimal)999.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Kogan", (decimal)299.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Sceptre", (decimal)399.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Viotek", (decimal)499.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Pixio", (decimal)599.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Nixeus", (decimal)699.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "BLU", (decimal)199.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "UMIDIGI", (decimal)149.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Doogee", (decimal)99.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Ulefone", (decimal)129.99),
                new Item(ItemCategory.Smartphone, Guid.NewGuid().ToString(), "Cubot", (decimal)79.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Xiaomi Mi Notebook", (decimal)1099.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Honor MagicBook", (decimal)1299.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Realme Book", (decimal)1499.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "RedmiBook", (decimal)1699.99),
                new Item(ItemCategory.Laptop, Guid.NewGuid().ToString(), "Huawei MateBook D", (decimal)1899.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Alldocube iPlay", (decimal)199.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Teclast P", (decimal)149.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Chuwi HiPad", (decimal)99.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Vankyo MatrixPad S", (decimal)129.99),
                new Item(ItemCategory.Tablet, Guid.NewGuid().ToString(), "Dragon Touch Max", (decimal)179.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "Azulle Byte", (decimal)599.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "KAMRUI", (decimal)699.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "AWOW", (decimal)799.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "CHUWI HeroBox", (decimal)899.99),
                new Item(ItemCategory.PC, Guid.NewGuid().ToString(), "T-Bao", (decimal)999.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Acer Nitro", (decimal)299.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Gigabyte G", (decimal)399.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "MSI MAG", (decimal)499.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Philips Momentum", (decimal)599.99),
                new Item(ItemCategory.Monitor, Guid.NewGuid().ToString(), "Samsung Space", (decimal)699.99)
            };
            await _itemsCollection.InsertManyAsync(items);
        }
    }
}
