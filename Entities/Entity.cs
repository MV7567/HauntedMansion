using HauntedMansion.Core;

namespace HauntedMansion.Entities
{
    /// <summary>
    /// Abstract base for all entities that can battle (player and enemies)
    /// </summary>
    public abstract class Entity
    {
        public string Name { get; init; }
        public int CurrentHP { get; private set; }      // current HP is only modified via TakeDamage() and Heal()
        protected CharacterStats Stats { get; init; }

        protected Entity(string name, CharacterStats baseStats)
        {
            Name = name;
            Stats = baseStats;
            CurrentHP = baseStats.MaxHP;
        }

        /// <summary>
        /// Calculates final stats with equipment and state modifiers
        /// Overwritten by Player and Enemy to apply their specific modifiers
        /// </summary>
        /// <returns></returns>
        public abstract CharacterStats GetEffectiveStats();

        public void TakeDamage(int amount)
        {
            if (amount < 0) return;
            CurrentHP = Math.Max(0, CurrentHP - amount);
        }

        public void Heal(int amount)
        {
            if (amount < 0) return;
            CurrentHP = Math.Min(Stats.MaxHP, CurrentHP + amount);
        }
        public bool IsAlive() => CurrentHP > 0;
    }
}