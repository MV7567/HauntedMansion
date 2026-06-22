using HauntedMansion.Entities;
using HauntedMansion.Inventory.Interfaces;

namespace HauntedMansion.Interactions
{
    /// <summary>
    /// Container with items and money
    /// Interact() transfers items to player inventory
    /// </summary>
    public class LootableObject : IInteractable
    {
        private readonly string _description;
        private readonly List<IItem> _containedItems;
        private readonly int _containedMoney;
        private bool _isLooted;

        public LootableObject(string description, List<IItem> items, int money)
        {
            _description = description;
            _containedItems = items ?? new List<IItem>();
            _containedMoney = money;
        }

        /// <summary>
        /// Transfers contents to player. Returns message for IRenderer.
        /// </summary>
        public string Interact(Player player)
        {
            if (_isLooted)
            {
                return "There's nothing left here.";;
            }

            if (_containedItems.Count == 0 && _containedMoney == 0)
            {
                _isLooted = true;
                return "It's empty.";
            }
            
            var messages = new List<string>();
            
            // transfer item to player inventory
            foreach (var item in _containedItems)
            {
                player.PlayerInventory.AddItem(item);
                messages.Add($"Found: {item.Name}");
            }
            
            if (_containedMoney > 0)
            {
                player.AddMoney(_containedMoney);
                messages.Add($"Found {_containedMoney} coins.");
            }
            _isLooted = true;
            return string.Join("\n", messages);
        }
        public string GetDescription() =>
            _isLooted ? $"{_description} (empty)" : _description;
    }
}