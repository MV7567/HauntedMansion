using HauntedMansion.Core;
using HauntedMansion.Inventory.Interfaces;

namespace HauntedMansion.Inventory
{
    /// <summary>
    /// Manages the equipment slots on the player
    /// Inventory tracks carried items
    /// EquipmentSlots tracks worn items
    /// GetTotalModifiers() called by Entity.GetEffectiveStats()
    /// to combine all bonuses for CombatEngine
    /// </summary>
    public class EquipmentSlots
    {
        // Dictionary ensures only ONE item can be equipped per slot type
        private Dictionary<EquipmentSlot, IEquippable> _slots = new();

        /// <summary>
        /// Places item in its designated slot.
        /// If slot already occupied, unequips existing item first.
        /// </summary>
        public void Equip(IEquippable item, Entities.Player player)
        {
            if (_slots.ContainsKey(item.Slot))
                Unequip(item.Slot, player);
            
            _slots[item.Slot] = item;
        }

        /// <summary>
        /// Empties the slot. Moves item back to player inventory.
        /// </summary>
        public void Unequip(EquipmentSlot slot, Entities.Player player)
        {
            if (!_slots.ContainsKey(slot)) return;
            var item = _slots[slot];
            _slots.Remove(slot);
            player.PlayerInventory.AddItem(item);
        }

        /// <summary>
        /// Sums StatModifiers from all occupied slots
        /// Called by Entity.GetEffectiveStats() to get total bonuses
        /// Returns empty StatModifier if no items equipped
        /// </summary>
        public StatModifier GetTotalModifiers()
        {
            var total = new StatModifier();

            foreach (var item in _slots.Values)
            {
                var mods = item.GetStatModifiers();
                total.AttackBonus   += mods.AttackBonus;
                total.DefenceBonus  += mods.DefenceBonus;
                total.MagicBonus    += mods.MagicBonus;
                total.SpeedBonus    += mods.SpeedBonus;
                total.AccuracyBonus += mods.AccuracyBonus;
            }

            return total;
        }

        /// <summary>
        /// Returns item in given slot (or null if empty)
        /// Used by UI to display items
        /// </summary>
        public IEquippable GetSlot(EquipmentSlot slot)
        {
            return _slots.TryGetValue(slot, out var item) ? item : null;
        }
    }
}