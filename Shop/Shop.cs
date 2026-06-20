using HauntedMansion.Entities;
using HauntedMansion.Inventory.Interfaces;

namespace HauntedMansion.Shop
{
    /// <summary>
    /// shop implementation
    /// Sell() validates funds and deducts from player money
    /// BuyFromPlayer() pays a fraction of the sell price
    /// </summary>
    public class Shop : IShop
    {
        private readonly List<IItem> _inventory = new();
        private readonly Dictionary<IItem, int> _prices = new();
        
        // buy back rate: get back half for half the price
        private const float BuyBackRate = 0.5f;

        public Shop(List<(IItem item, int price)> stock)
        {
            foreach (var (item, price) in stock)
            {
                _inventory.Add(item);
                _prices[item] = price;
            }
        }

        /// <summary>
        /// validates player selected index and funds, deducts price from money, adds item to inventory
        /// </summary>
        public IItem Sell(int selection, Player player)
        {
            if (selection < 0 || selection >= _inventory.Count)
            {
                Console.WriteLine("Invalid selection.");
                return null;
            }

            var item = _inventory[selection];
            int price = _prices[item];

            if (player.Money < price)
            {
                Console.WriteLine($"Not enough money. {item.Name} costs {price}.");
                return null;
            }
            
            player.AddMoney(-price);
            _inventory.Remove(item);
            _prices.Remove(item);
            
            Console.WriteLine($"Bought {item.Name} for {price} coins.");
            return item;
        }

        /// <summary>
        /// buy item from player at a reduced rate, removes item form player inventory, adds money
        /// </summary>
        public void BuyFromPlayer(IItem item, Player player)
        {
            int basePrice = _prices.TryGetValue(item, out int p) ? p : 10;
            int buyBackPrice = (int)(basePrice * BuyBackRate);

            player.PlayerInventory.RemoveItem(item);
            player.AddMoney(buyBackPrice);
            
            // restock the item
            _inventory.Add(item);
            _prices[item] = basePrice;
            
            Console.WriteLine($"Sold {item.Name} for {buyBackPrice} coins.");
        }

        public List<(IItem item, int price)> GetStock()
        {
            return _inventory.Select(item => (item, _prices[item])).ToList();

        }
    }
}
