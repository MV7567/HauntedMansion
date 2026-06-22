using HauntedMansion.Combat;
using HauntedMansion.Entities;

namespace HauntedMansion.Dialogue.Actions
{
    /// <summary>
    /// Changes enemy state flag and takes away the appropriate item.
    /// </summary>
    public class ItemAndStateDialogueAction(Enemy enemy, string targetStateName, string keyId, Player player, string message = "") : IDialogueAction
    {
        public string Execute(CombatContext? context)
        {
            // If the action requires transitioning to "Sparable", set the flag
            if (targetStateName.Contains("Sparable", StringComparison.OrdinalIgnoreCase))
            {
                enemy.IsSparable = true;
            }
            
            var item = player.PlayerInventory.GetKeyItem().FirstOrDefault(k => k.KeyID == keyId);
            if (item != null)
            {
                player.PlayerInventory.RemoveItem(item);
            }
            
            return message;
        }
    }
}