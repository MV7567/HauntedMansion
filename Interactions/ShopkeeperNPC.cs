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

        public void Interact(Player player)
        {
            // future: open shop UI via IRenderer
            Console.WriteLine($"{Name}: Welcome! What are you buying?");
        }

        public string GetDescription()
        {
            return $"{Name} is here. Talk to them to buy stuff.";
        }
    }
}