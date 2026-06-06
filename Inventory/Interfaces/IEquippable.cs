using HauntedMansion.Core;

namespace HauntedMansion.Inventory.Interfaces
{
    /// <summary>
    /// Extends IItem with equip logic
    /// </summary>
    public interface IEquippable : IItem
    {
        EquipmentSlot Slot { get; }
        StatModifier GetStatModifiers();
        void Equip(Entities.Player player);
        void Unequip(Entities.Player player);
    }
    public enum EquipmentSlot { Weapon, Spell, Head, Torso, Talisman }
}