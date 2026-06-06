using HauntedMansion.Core;
using HauntedMansion.Inventory.Items;
using HauntedMansion.Inventory;

namespace HauntedMansion.Entities
{
    /// <summary>
    /// Main character. Owns Inventory and EquipmentSlots
    /// </summary>
    public class Player : Entity
    {
        public int Experience { get; private set; }
        public int Money { get; private set; }
        
        public Inventory.Inventory PlayerInventory { get; init; }
        public EquipmentSlots Equipment { get; init; }

        // money and experience start as 0
        public Player(string name, CharacterStats baseStats) : base(name, baseStats)
        {
            PlayerInventory = new Inventory.Inventory();
            Equipment = new EquipmentSlots();
        }

        public void GainExperience(int amount)
        {
            if (amount > 0) Experience += amount;
        }
        public void AddMoney(int amount)
        {
            Money += amount;
            if (Money < 0) Money = 0; // no negative money
        }
        
        /// <summary>
        /// Base stats + equipment modifiers
        /// Called by combat engine instead of stats directly
        /// </summary>
        public override CharacterStats GetEffectiveStats()
        {
            var mods = Equipment.GetTotalModifiers();
            return new CharacterStats(
                Stats.Attack + mods.AttackBonus,
                Stats.Defence + mods.DefenceBonus,
                Stats.Magic + mods.MagicBonus,
                Stats.Speed + mods.SpeedBonus,
                Stats.Accuracy + mods.AccuracyBonus,
                Stats.MaxHP
            );
        }
    }
}