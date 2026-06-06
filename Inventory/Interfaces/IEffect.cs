using HauntedMansion.Entities;

namespace HauntedMansion.Inventory.Interfaces
{
    /// <summary>
    /// Effects caused by consumable
    /// </summary>
    public interface IEffect
    {
        void Apply(Entity target);
    }
}