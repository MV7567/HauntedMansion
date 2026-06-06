namespace HauntedMansion.Inventory.Interfaces
{
    /// <summary>
    /// Base for all items in the game
    /// Use() returns true if item should be removed from inventory after use
    /// false if item should be kept (wrong context or reusable item)
    /// </summary>
    public interface IItem
    {
        string Name { get; }
        string Description { get; }
        bool Use(Entities.Player player);
    }
}