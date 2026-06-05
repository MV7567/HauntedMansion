using HauntedMansion.Core;
using HauntedMansion.Inventory.Items;

namespace HauntedMansion.Entities
{
    /// <summary>
    /// Main character. Owns Inventory and EquipmentSlots
    /// </summary>
    public class Player : Entity
    {
        public int Experience { get; private set; }
        public int Money { get; private set; }
        
        // to be added
        //public Inventory PlayerInventory { get; private set; }
        //public EquipmentSlots Equipment { get; private set; }

        // money and experience start as 0
        public Player(string name, CharacterStats baseStats) : base(name, baseStats) {}

        public void GainExperience(int amount)
        {
            if (amount > 0) Experience += amount;
        }
        public void AddMoney(int amount)
        {
            Money += amount;
            if (Money < 0) Money = 0; // no negative money
        }
        
        public override CharacterStats GetEffectiveStats()
        {
            // future: combine base stats with modifiers from equipment!!!!!!!!
            return Stats;
        }
    }
}