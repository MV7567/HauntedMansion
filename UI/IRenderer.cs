using HauntedMansion.Combat;
using HauntedMansion.Dialogue;
using HauntedMansion.Entities;
using HauntedMansion.Inventory.Interfaces;
using HauntedMansion.Shop;
using HauntedMansion.World;

namespace HauntedMansion.UI
{
    /// <summary>
    /// Display logic, prints to the console
    /// </summary>
    public interface IRenderer
    {
        void RenderRoom(IRoom room, Player player, string description);
        void RenderCombat(CombatContext context);
        void RenderMenu(string title, List<string> options);
        void RenderDialogue(DialogueNode node);
        void RenderInventory(Player player);
        void RenderEquipScreen(Player player);
        void RenderShop(List<(IItem item, int price)> stock, Player player);
        void RenderMessage(string message);
        
        void RenderPassageBlocked(string message);
        void RenderCombatResult(CombatResult result);
        void RenderStateChange(string message);
        void RenderSaveConfirmation();
        void RenderError(string message);
        void RenderInteractionResult(string message);

        void ClearScreen();
    }
}