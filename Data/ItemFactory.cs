using HauntedMansion.Core;
using HauntedMansion.Inventory;
using HauntedMansion.Inventory.Interfaces;
using HauntedMansion.Inventory.Items;

namespace HauntedMansion.Data
{
    /// <summary>
    /// Creates IItem instances from item ID strings.
    /// Used by InteractableFactory and RoomFactory.
    /// New item types added by extending the switch expression (OCP).
    /// </summary>
    public class ItemFactory
    {
        private readonly IContentLoader _loader;

        public ItemFactory(IContentLoader loader)
        {
            _loader = loader;
        }

        /// <summary>
        /// Creates item from prefixed ID string.
        /// Format: "consumable:healing_potion", "equipment:rusty_sword",
        ///         "key:old_key", or bare "healing_potion" (treated as consumable)
        /// </summary>
        public IItem? CreateItem(string itemId)
        {
            if (itemId.StartsWith("consumable:"))
                return CreateConsumable(itemId["consumable:".Length..]);
            
            if (itemId.StartsWith("equipment:"))
                return CreateEquipment(itemId["equipment:".Length..]);
            
            if (itemId.StartsWith("key:"))
                return CreateKeyItem(itemId["key:".Length..]);
            
            // Default: treat as consumable
            return CreateConsumable(itemId);
        }

        public Consumable CreateConsumable(string itemId)
        {
            var (healAmount, name) = itemId switch
            {
                "healing_potion"       => (30, "Healing Potion"),
                "large_healing_potion" => (60, "Large Healing Potion"),
                _ => (20, itemId)
            };
            
            return new Consumable(
                name, 
                _loader.GetItemDescription(itemId),
                new HealEffect(healAmount));
        }

        public Equipment CreateEquipment(string itemId)
        {
            var (name, slot, mods) = itemId switch
            {
                "rusty_sword"    => ("Rusty Sword", EquipmentSlot.Weapon,
                    new StatModifier(8, 0, 0, 2, 5)),
                "iron_shield"    => ("Iron Shield", EquipmentSlot.Torso,
                    new StatModifier(0, 15, 0, -2, 0)),
                "magic_talisman" => ("Magic Talisman", EquipmentSlot.Talisman,
                    new StatModifier(0, 0, 20, 0, 5)),
                "leather_helm"   => ("Leather Helm", EquipmentSlot.Head,
                    new StatModifier(0, 5, 0, 0, 10)),
                "magic_staff"    => ("Magic Staff", EquipmentSlot.Weapon,
                    new StatModifier(0, 0, 15, -2, 0)),
                _ => ("Unknown Item", EquipmentSlot.Weapon, new StatModifier())
            };
            
            return new Equipment(
                name,
                _loader.GetItemDescription(itemId),
                slot,
                mods);
        }
        
        public KeyItem CreateKeyItem(string keyId)
        {
            var (name, desc) = keyId switch
            {
                "old_key"    => ("Old Key",
                    "A heavy iron key. It might open something."),
                "teddy_bear" => ("Teddy Bear",
                    "A worn stuffed bear. Someone must miss it."),
                _ => (keyId, _loader.GetItemDescription(keyId))
            };

            return new KeyItem(name, desc, keyId);
        }
    }
}
