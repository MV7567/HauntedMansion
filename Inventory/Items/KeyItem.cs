using HauntedMansion.Entities;
using HauntedMansion.Inventory.Interfaces;

namespace HauntedMansion.Inventory.Items
{
    /// <summary>
    /// Story or puzzle item that cannot be discarded.
    /// Use() returns false if used in wrong context (item kept),
    /// true only when consumed by a valid interaction. 
    /// </summary>
    public class KeyItem : IItem
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public string KeyID { get; init; }

        public KeyItem(string name, string description, string keyId)
        {
            Name = name;
            Description = description;
            KeyID = keyId;
        }

        /// <summary>
        /// Context validation handled by the interaction that consumes this item
        /// (eg DialogueChoice action or LootableObject).
        /// Returns false by default - item only removed when explicitly consumed.
        /// </summary>
        public bool Use(Player player)
        {
            return false;
        }
    }
}