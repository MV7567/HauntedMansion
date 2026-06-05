namespace HauntedMansion.Core
{
    /// <summary>
    /// Stores the base statistics of an entity
    /// These values represent unmodified stats before any equipment bonuses.
    /// Never modified directly during gameplay - use StatModifier for bonuses.
    /// </summary>
    public class CharacterStats
    {
        public int Attack { get; init; }
        public int Defence { get; init; }
        public int Magic { get; init; }
        public int Speed { get; init; }
        public int Accuracy { get; init; }
        public int MaxHP { get; init; }

        public CharacterStats(int attack, int defence, int magic,
                              int speed, int accuracy, int maxHP)
        {
            Attack = attack;
            Defence = defence;
            Magic = magic;
            Speed = speed;
            Accuracy = accuracy;
            MaxHP = maxHP;
        }
    }
}