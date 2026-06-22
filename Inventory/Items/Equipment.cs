using HauntedMansion.Core;
using HauntedMansion.Entities;
using HauntedMansion.Inventory.Interfaces;

namespace HauntedMansion.Inventory.Items
{
    /// <summary>
    /// Wearable item providing stat bonuses via StatModifier
    /// Use() delegates to Equip() - equipping counts as using the item.
    /// </summary>
    public class Equipment : IEquippable
    {
        public string ID { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public EquipmentSlot Slot { get; init; }
        private StatModifier StatMods { get; init; }

        public Equipment(string id, string name, string description,
                         EquipmentSlot slot, StatModifier statMods)
        {
            ID = id;
            Name = name;
            Description = description;
            Slot = slot;
            StatMods = statMods;
        }

        public StatModifier GetStatModifiers() => StatMods;

        /// <summary>
        /// Using equipment equips it directly.
        /// Returns true to move item from inventory to EquipmentSlots.
        /// </summary>
        public bool Use(Player player)
        {
            Equip(player);
            return true;
        }

        public void Equip(Player player)
        {
            player.Equipment.Equip(this, player);
        }

        public void Unequip(Player player)
        {
            player.Equipment.Unequip(this.Slot, player);
        }
    }
}