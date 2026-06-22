using HauntedMansion.Entities;
using HauntedMansion.Inventory.Interfaces;

namespace HauntedMansion.Inventory.Items
{
    /// <summary>
    /// Single or multi-use item with a variable effect
    /// Use() calls Effect.Apply(player) then returns true to remove from inventory
    /// </summary>
    public class Consumable : IItem
    {
        public string ID { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        private IEffect Effect { get; init; }

        public Consumable(string id, string name, string description, IEffect effect)
        {
            ID = id;
            Name = name;
            Description = description;
            Effect = effect;
        }

        public bool Use(Player player)
        {
            Effect.Apply(player);
            return true;        // consumed - remove from inventory
        }
    }
}