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
            _isLooted = false;
        }

        public void Interact(Player player)
        {
            if (_isLooted)
            {
                Console.WriteLine("There's nothing left here.");
                return;
            }

            if (_containedItems.Count == 0 && _containedMoney == 0)
            {
                Console.WriteLine("It's empty.");
                _isLooted = true;
                return;
            }
            
            // transfer item to player inventory
            foreach (var item in _containedItems)
            {
                player.PlayerInventory.AddItem(item);
                Console.WriteLine($"Found {_containedMoney} coins.");
            }

            _isLooted = true;
        }

        public string GetDescription()
        {
            return _isLooted ? $"{_description} (empty)" : _description;
        }
    }
}