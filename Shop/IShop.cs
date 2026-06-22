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
        // Returns purchased item and message, or null item if failed.
        (IItem? item, string message) Sell(int selection, Player player);
        
        // Returns confirmation message.
        string BuyFromPlayer(IItem item, Player player);
        
        List<(IItem item, int price)> GetStock();
    }
}