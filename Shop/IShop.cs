using HauntedMansion.Entities;
using HauntedMansion.Inventory.Interfaces;

namespace HauntedMansion.Shop
{
    /// <summary>
    /// interface for all shop implmentations
    /// ShopkeeperNPC depends on this interface not Shop directly
    /// </summary>
    public interface IShop
    {
        /// <summary>
        /// Sells item at specific index to player
        /// returns null if insuffiecient funds or wrong selection
        /// </summary>
        IItem Sell(int selection, Player player);
        
        void BuyFromPlayer(IItem item, Player player);

        /// <summary>
        /// All items available for purchase
        /// </summary>
        List<(IItem item, int price)> GetStock();
    }
}