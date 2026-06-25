using HauntedMansion.Combat;
using HauntedMansion.Dialogue;
using HauntedMansion.Entities;
using HauntedMansion.Inventory.Interfaces;
using HauntedMansion.World;

namespace HauntedMansion.UI
{
    /// <summary>
    /// text based implementation of irender, can be swapped to graphical renderer game logic
    /// </summary>
    public class TextRenderer : IRenderer
    {
        public void RenderRoom(IRoom room, Player player, string description)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine($" {room.GetRoomID().ToUpper().Replace('_', ' ')}");
            Console.WriteLine(new string('=', 40));

            Console.WriteLine($" {description}\n");
            
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
                Console.WriteLine($" {i + 1}. {node.Choices[i].Text}");
            
            if (node.Choices.Count > 0)
                Console.Write("\nChoice: ");
        }

        public void RenderInventory(Player player)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine(" INVENTORY & STATS");
            Console.WriteLine(new string('=', 40));
            
            var stats = player.GetEffectiveStats();
            Console.WriteLine($" Name:   {player.Name}");
            Console.WriteLine($" HP:     {player.CurrentHP} / {stats.MaxHP}");
            Console.WriteLine($" Attack: {stats.Attack}");
            Console.WriteLine($" Def:    {stats.Defence}");
            Console.WriteLine($" Magic:  {stats.Magic}");
            Console.WriteLine($" Speed:  {stats.Speed}");
            Console.WriteLine($" Acc:    {stats.Accuracy}");
            Console.WriteLine($" Coins:  {player.Money}");
            Console.WriteLine(new string('-', 40));
            
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
        
        public void RenderEquipScreen(Player player)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine("  EQUIP");
            Console.WriteLine(new string('=', 40));

            var equippables = player.PlayerInventory.GetEquippables();
            if (equippables.Count == 0)
            {
                Console.WriteLine("  No equipment in inventory.");
                return;
            }

            Console.WriteLine("\n  Equipment in inventory:");
            for (int i = 0; i < equippables.Count; i++)
            {
                var e = equippables[i];
                var mods = e.GetStatModifiers();
                Console.WriteLine($"  {i + 1}. {e.Name} [{e.Slot}]");
                Console.WriteLine($"     ATK:{mods.AttackBonus:+#;-#;0} " +
                                  $"DEF:{mods.DefenceBonus:+#;-#;0} " +
                                  $"MAG:{mods.MagicBonus:+#;-#;0} " +
                                  $"SPD:{mods.SpeedBonus:+#;-#;0} " +
                                  $"ACC:{mods.AccuracyBonus:+#;-#;0}");
            }

            Console.WriteLine("\n  Currently equipped:");
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                var equipped = player.Equipment.GetSlot(slot);
                Console.WriteLine(equipped != null
                    ? $"  {slot,-12}: {equipped.Name}"
                    : $"  {slot,-12}: (empty)");
            }
        }

        public void RenderShop(List<(IItem item, int price)> stock, Player player)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine(" SHOP");
            Console.WriteLine(new string('=', 40));
            Console.WriteLine($"  Your coins: {player.Money}\n");

            if (stock.Count == 0)
            {
                Console.WriteLine("  Nothing for sale.");
            }
            else
            {
                for (int i = 0; i < stock.Count; i++)
                {
                    var (item, price) = stock[i];
                    Console.WriteLine($"  {i + 1}. {item.Name} - {price} coins");
                    Console.WriteLine($"     {item.Description}");
                }
            }
            Console.WriteLine($"  {stock.Count + 1}. Sell an item");
            Console.WriteLine($"  {stock.Count + 2}. Leave");
        }

        public void RenderMessage(string message)
        {
            Console.WriteLine($"\n  {message}");
        }
        
        public void RenderPassageBlocked(string message)
        {
            Console.WriteLine($"\n  [!] {message}");
        }
        
        public void RenderCombatResult(CombatResult result)
        {
            Console.WriteLine(result.WasHit
                ? $"\n  >> {result.Message}"
                : $"\n  -- {result.Message}");
        }
        
        public void RenderStateChange(string message)
        {
            Console.WriteLine($"\n  * {message}");
        }
        
        public void RenderSaveConfirmation()
        {
            Console.WriteLine("\n  Game saved.");
        }
        
        public void RenderError(string message)
        {
            Console.WriteLine($"\n  [ERROR] {message}");
        }
        
        public void RenderInteractionResult(string message)
        {
            if (!string.IsNullOrEmpty(message))
                Console.WriteLine($"\n  {message}");
        }
        
        public void RenderContinuePrompt()
        {
            Console.WriteLine("\n  [Press Enter to continue...]");
        }
        
        public void ClearScreen()
        {
            Console.Clear();
        }
    }
}