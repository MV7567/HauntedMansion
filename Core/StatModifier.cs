namespace HauntedMansion.Core
{
    /// <summary>
    /// Represents temporary stat bonuses applied by equipped items.
    /// Combined with CharacterStats by Entity.GetEffectiveStats() at runtime.
    /// Allows armor-piercing attacks by bypassing this and using base stats directly.
    /// </summary>
    public class StatModifier
    {
        public int AttackBonus { get; set; }
        public int DefenceBonus { get; set; }
        public int MagicBonus { get; set; }
        public int SpeedBonus { get; set; }
        public int AccuracyBonus { get; set; }
        
        // Default constructor, all bonuses start at 0
        public StatModifier() {}

        // full constructor used by factories when reading item stats from hson
        public StatModifier(int attackBonus, int defenceBonus, int magicBonus,
                             int speedBonus, int accuracyBonus)
        {
            AttackBonus = attackBonus;
            DefenceBonus = defenceBonus;
            MagicBonus = magicBonus;
            SpeedBonus = speedBonus;
            AccuracyBonus = accuracyBonus;
        }
    }
}