using HauntedMansion.Entities;
using HauntedMansion.Shop;

namespace HauntedMansion.Interactions
{
    /// <summary>
    /// NPC separate from Shop
    /// Interact() opens the shop interface
    /// </summary>
    public class ShopkeeperNPC : IInteractable
    {
        public string Name { get; init; }
        private readonly IShop _shopModule;

        public ShopkeeperNPC(string name, IShop shopModule)
        {
            Name = name;
            _shopModule = shopModule;
        }

        public string Interact(Player player)
        {
            // Returns signal string - ExplorationGameState checks
            // if interactable is ShopkeeperNPC and opens shop UI
            return $"OPEN_SHOP:{Name}";
        }

        public string GetDescription() =>
            $"{Name} is here. Talk to them to open the shop.";
        
        public IShop GetShop() => _shopModule;
    }
}