using HauntedMansion.Entities;
using HauntedMansion.Inventory.Interfaces;

namespace HauntedMansion.Inventory
{
    /// <summary>
    /// Restores HP to target entity.
    /// </summary>
    public class HealEffect : IEffect
    {
        private readonly int _amount;
        
        public HealEffect(int amount)
        {
            _amount = amount;
        }
        
        public void Apply(Entity target)
        {
            target.Heal(_amount);
        }
    }
}