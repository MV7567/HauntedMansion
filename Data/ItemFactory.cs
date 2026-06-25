using HauntedMansion.Core;
using HauntedMansion.Inventory.Interfaces;
using HauntedMansion.Inventory.Items;
using HauntedMansion.Inventory;

namespace HauntedMansion.Data
{
    /// <summary>
    /// Creates IItem instances dynamically from items.json data.
    /// </summary>
    public class ItemFactory
    {
        private readonly IContentLoader _loader;

        public ItemFactory(IContentLoader loader)
        {
            _loader = loader;
        }

        public IItem? CreateItem(string itemId)
        {
            // find the actual ID
            var rawId = itemId;
            if (itemId.StartsWith("consumable:")) rawId = itemId["consumable:".Length..];
            else if (itemId.StartsWith("equipment:")) rawId = itemId["equipment:".Length..];
            else if (itemId.StartsWith("key:")) rawId = itemId["key:".Length..];

            var data = _loader.GetItemData(rawId);
            if (data == null) return new KeyItem(rawId, "Unknown Item", "Unknown", rawId);

            string name = data.Name ?? rawId;
            string desc = data.Description ?? "";

            if (itemId.StartsWith("key:"))
                return new KeyItem(itemId, name, desc, rawId);

            if (itemId.StartsWith("equipment:"))
            {
                var slotStr = data.Slot ?? "";
                var slot = Enum.TryParse<EquipmentSlot>(slotStr, true, out var parsedSlot) ? parsedSlot : EquipmentSlot.Weapon;
                var stats = data.Stats != null ? new StatModifier(data.Stats.Attack, data.Stats.Defence, data.Stats.Magic, data.Stats.Speed, data.Stats.Accuracy) : new StatModifier();
                return new Equipment(itemId, name, desc, slot, stats);
            }

            return new Consumable(itemId, name, desc, new HealEffect(data.HealAmount));
        }
    }
}