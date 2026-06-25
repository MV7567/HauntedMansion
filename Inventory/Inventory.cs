using HauntedMansion.Inventory.Interfaces;
using HauntedMansion.Inventory.Items;

namespace HauntedMansion.Inventory
{
    /// <summary>
    /// Manages the players carried items split into 3 lists
    /// AddItem() determines correct list by item type
    /// </summary>
    public class Inventory
    {
        private List<Consumable> _consumables = new();
        private List<KeyItem> _keyItems = new();
        private List<IEquippable> _equippables = new();

        /// <summary>
        /// Determines item type and adds to correct list.
        /// Called by LootableObject.Interact() and Shop.Sell()
        /// </summary>
        public void AddItem(IItem item)
        {
            switch (item)
            {
                case Consumable c:
                    _consumables.Add(c);
                    break;
                case KeyItem k:
                    _keyItems.Add(k);
                    break;
                case IEquippable e:
                    _equippables.Add(e);
                    break;
                default:
                    throw new ArgumentException($"Unknown item type {item.GetType().Name}");
            }
        }

        /// <summary>
        /// Remove item from list.
        /// Called after Use() returns true or when selling to shop
        /// </summary>
        public void RemoveItem(IItem item)
        {
            switch (item)
            {
                case Consumable c:
                    _consumables.Remove(c);
                    break;
                case KeyItem k:
                    _keyItems.Remove(k);
                    break;
                case IEquippable e:
                    _equippables.Remove(e);
                    break;
            }
        }
        
        // Expose specific lists for the UI Renderer to display
        public List<Consumable> GetConsumables() => _consumables;
        public List<KeyItem> GetKeyItem() => _keyItems;
        public List<IEquippable> GetEquippables() => _equippables;

        /// <summary>
        /// Checks if inventory contains a KeyItem with given ID.
        /// Used by DialogueChoice actions and puzzle interactions
        /// </summary>
        public bool HasKeyItem(string keyId)
        {
            string cleanId = keyId.Replace("key:", "");
            return _keyItems.Any(k => k.ID.Replace("key:", "").Equals(cleanId, StringComparison.OrdinalIgnoreCase));        
        }
    }
}