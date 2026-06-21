using HauntedMansion.Combat;
using HauntedMansion.Dialogue;
using HauntedMansion.Entities;
using HauntedMansion.Inventory.Interfaces;
using HauntedMansion.Shop;
using HauntedMansion.World;

namespace HauntedMansion.UI
{
    /// <summary>
    /// text based implementation of irender, can be swapped to graphical renderer game logic
    /// </summary>
    public class TextRenderer : IRenderer
    {
        public void RenderRoom(IRoom room, Player player)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine($" {room.GetRoomID().ToUpper().Replace('_', ' ')}");
            Console.WriteLine(new string('=', 40));

            var enemies = room.GetEnemies();
            if (enemies.Count > 0)
            {
                Console.WriteLine("\nEnemies:");
                foreach (var e in enemies)
                    Console.WriteLine($" - {e.Name} (HP: {e.CurrentHP})");
            }

            var interactables = room.GetInteractables();
            if (interactables.Count > 0)
            {
                Console.WriteLine("\nObjects:");
                foreach (var i in interactables)
                    Console.WriteLine($"    - {i.GetDescription()}");
            }
            Console.WriteLine($"\nCoins: {player.Money} HP: {player.CurrentHP}");
        }

        public void RenderCombat(CombatContext context)
        {
            Console.WriteLine("\n" + new string('-', 40));
            Console.WriteLine(" COMBAT");
            Console.WriteLine(new string('-', 40));
            Console.WriteLine($" Turn: {context.TurnNumber}");
            Console.WriteLine($" Your HP: {context.Player.CurrentHP}");
            
            Console.WriteLine("\n Enemies:");
            foreach (var e in context.Enemies)
            {
                Console.WriteLine($"    - {e.Name} (HP: {e.CurrentHP})");
                var parts = new List<string>();
                foreach (BodyPartType type in Enum.GetValues(typeof(BodyPartType)))
                {
                    var part = e.GetBodyPart(type);
                    if (part != null)
                        parts.Add(part.IsDisabled ? $"{type}[DISABLED]" : $"{type}");
                }

                Console.WriteLine($" Parts: {string.Join(", ", parts)}");
            }
        }

        public void RenderMenu(string title, List<string> options)
        {
            Console.WriteLine($"\n{title}");
            for (int i = 0; i < options.Count; i++)
                Console.WriteLine($" {i + 1}. {options[i]}");
            Console.Write("\nChoice: ");
        }

        public void RenderDialogue(DialogueNode node)
        {
            Console.WriteLine($"\n\" {node.Text}\"");
            for (int i = 0; i < node.Choices.Count; i++)
                Console.WriteLine($" {i + i}. {node.Choices[i].Text}");
            
            if (node.Choices.Count > 0)
                Console.Write("\nChoice: ");
        }

        public void RenderInventory(Player player)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine(" INVENTORY");
            Console.WriteLine(new string('=', 40));
            Console.WriteLine($" Coins: {player.Money}");
            
            var consumables = player.PlayerInventory.GetConsumables();
            if (consumables.Count > 0)
            {
                Console.WriteLine("\n  Consumables:");
                foreach (var c in consumables)
                    Console.WriteLine($"    - {c.Name}: {c.Description}");
            }

            var keyItems = player.PlayerInventory.GetKeyItem();
            if (keyItems.Count > 0)
            {
                Console.WriteLine("\n  Key Items:");
                foreach (var k in keyItems)
                    Console.WriteLine($"    - {k.Name}: {k.Description}");
            }

            var equippables = player.PlayerInventory.GetEquippables();
            if (equippables.Count > 0)
            {
                Console.WriteLine("\n  Equipment:");
                foreach (var e in equippables)
                    Console.WriteLine($"    - {e.Name}: {e.Description}");
            }
            
            Console.WriteLine("\n  Equipped:");
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                var equipped = player.Equipment.GetSlot(slot);
                Console.WriteLine(equipped != null
                    ? $"    {slot}: {equipped.Name}"
                    : $"    {slot}: (empty)");
            }
        }

        public void RenderShop(List<(IItem item, int price)> stock, Player player)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine(" SHOP");
            Console.WriteLine(new string('=', 40));
            Console.WriteLine($"  Your gold: {player.Money}\n");

            if (stock.Count == 0)
            {
                Console.WriteLine("  Nothing for sale.");
                return;
            }

            for (int i = 0; i < stock.Count; i++)
            {
                var (item, price) = stock[i];
                Console.WriteLine($"  {i + 1}. {item.Name} - {price} coins");
                Console.WriteLine($"     {item.Description}");
            }
        }

        public void RenderMessage(string message)
        {
            Console.WriteLine($"\n  {message}");
        }
    }
}