using HauntedMansion.Interactions;
using HauntedMansion.Inventory.Interfaces;
using HauntedMansion.Shop;

namespace HauntedMansion.Data
{
    /// <summary>
    /// Creates IInteractable instances from JSON data.
    /// Separated from RoomFactory
    /// different objects (Lootables, Traps, NPCs, Shops) 
    /// behind a single interface
    /// </summary>
    public class InteractableFactory
    {
        private readonly IContentLoader _loader;
        private readonly ItemFactory _itemFactory;
        
        public InteractableFactory(IContentLoader loader, ItemFactory itemFactory)
        {
            _loader = loader;
            _itemFactory = itemFactory;
        }

        /// <summary>
        /// Builds all interactables for a given room from JSON.
        /// </summary>
        public List<IInteractable> CreateForRoom(string roomId)
        {
            var dataList = _loader.GetInteractables(roomId);
            return dataList
                .Select(CreateFromData)
                .Where(i => i != null)
                .Cast<IInteractable>()
                .ToList();
        }

        private IInteractable? CreateFromData(JsonContentLoader.InteractableData data)
        {
            // specific subclasses based on the JSON Type field
            return data.Type switch
            {
                "lootable" => CreateLootable(data),
                "trap" => CreateTrap(data),
                "shop" => CreateShop(data),
                "npc" => CreateNPC(data),
                _ => null
            };
        }

        private LootableObject CreateLootable(JsonContentLoader.InteractableData data)
        {
            // Delegates the creation of items to the ItemFactory
            var items = data.Items
                .Select(id => _itemFactory.CreateItem(id))
                .Where(i => i != null)
                .Cast<IItem>()
                .ToList();

            return new LootableObject(
                data.Description ?? "An object",
                items,
                data.Money);
        }

        private Trap CreateTrap(JsonContentLoader.InteractableData data)
        {
            return new Trap(
                data.Description ?? "A trap",
                data.Damage);
        }

        private ShopkeeperNPC CreateShop(JsonContentLoader.InteractableData data)
        {
            var stock = data.Stock
                .Select(s =>
                {
                    var item = _itemFactory.CreateItem(s.Item ?? "");
                    return item == null
                        ? ((IItem, int)?)null
                        : ((IItem, int)?)(item, s.Price);
                })
                .Where(s => s != null)
                .Select(s => s!.Value)
                .ToList();
            
            var shop = new Shop.Shop(stock);
            return new ShopkeeperNPC(data.NpcName ?? "Shopkeeper", shop);
        }
        
        private NormalNPC CreateNPC(JsonContentLoader.InteractableData data)
        {
            return new NormalNPC(
                data.NpcName ?? "NPC",
                data.Description ?? "",
                data.NpcName?.ToLower().Replace(" ", "_") ?? "");
        }
    }
}