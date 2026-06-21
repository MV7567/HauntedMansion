using HauntedMansion.Combat;
using HauntedMansion.Dialogue;
using HauntedMansion.Entities;
using HauntedMansion.Shop;
using HauntedMansion.World;
using HauntedMansion.Inventory.Interfaces;

namespace HauntedMansion.UI
{
    /// <summary>
    /// Display logic, prints to the console
    /// </summary>
    public interface IRenderer
    {
        void RenderRoom(IRoom room, Player player);
        void RenderCombat(CombatContext context);
        void RenderMenu(string title, List<string> options);
        void RenderDialogue(DialogueNode node);
        void RenderInventory(Player player);
        void RenderShop(List<(IItem item, int price)> stock, Player player);
        void RenderMessage(string message);
    }
}