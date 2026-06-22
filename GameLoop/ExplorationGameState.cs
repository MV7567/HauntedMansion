using HauntedMansion.Data;
using HauntedMansion.Dialogue;
using HauntedMansion.Entities;
using HauntedMansion.Interactions;
using HauntedMansion.UI;
using HauntedMansion.UI.Commands;
using HauntedMansion.World;

namespace HauntedMansion.GameLoop
{
    /// <summary>
    /// player movement and room interaction
    /// calls try trigger encounter on each move
    /// switches to combat game state if encounter happens
    /// </summary>
    public class ExplorationGameState : IGameState
    {
        private readonly GameManager _manager;
        private readonly IContentLoader _loader;
        private readonly DialogueEngine _dialogueEngine;

        public ExplorationGameState(GameManager manager, IContentLoader loader)
        {
            _manager = manager;
            _loader = loader;
            _dialogueEngine = new DialogueEngine(loader);
        }

        public void OnEnter()
        {
            ShowExplorationMenu();
        }
        
        public void OnExit() { }
        
        public void HandleInput(ICommand command)
        {
            command.Execute();
        }

        /// <summary>
        /// show available actions in current room
        /// reads player input and creates command
        /// </summary>
        private void ShowExplorationMenu()
        {
            while (true)
            {
                _manager.Renderer.ClearScreen();
                var room = _manager.Map.GetCurrentRoom();
                var descr = _loader.GetRoomDescription(room.GetRoomID());
                
                _manager.Renderer.RenderRoom(room, _manager.Player, descr);
                
                var neighbours = _manager.Map.GetNeighbours(room.GetRoomID());
                var interactables = room.GetInteractables();

                var options = new List<string>();
                
                // movment options
                foreach (var neighbour in neighbours)
                    options.Add($"Go to {neighbour.GetRoomID().Replace('_', ' ')}");
                
                // Attack options if enemies present
                var enemies = room.GetEnemies();
                if (enemies.Count > 0)
                    options.Add($"Attack! ({enemies.Count} " +
                                $"{(enemies.Count == 1 ? "enemy" : "enemies")} present)");
                
                // interaction option
                foreach (var obj in interactables)
                    options.Add($"Examine: {obj.GetDescription()}");
                
                // always available
                options.Add("Open inventory");
                options.Add("Save game");
                options.Add("Quit");
                
                _manager.Renderer.RenderMenu("What do you do?", options);

                if (!int.TryParse(Console.ReadLine(), out int choice) ||
                    choice < 1 || choice > options.Count)
                {
                    _manager.Renderer.RenderMessage("Invalid choice");
                    continue;
                }

                choice--;
                
                // Handle movement
                if (choice < neighbours.Count)
                {
                    var target = neighbours[choice];
                    var moveCmd = new MoveCommand(
                        target.GetRoomID(), _manager.Map,
                        _manager.Player, _manager.Renderer);
                    moveCmd.Execute();

                    // Check for random encounter
                    // load new room to get the correct encounters
                    var currentRoom = _manager.Map.GetCurrentRoom();
                    if (room is Room concreteRoom)
                    {
                        var encountered = concreteRoom
                            .TryTriggerEncounter(_manager.Player);
                        if (encountered != null)
                        {
                            _manager.Renderer.RenderMessage(
                                $"\nA {encountered.Name} appears!");
                            _manager.ChangeState(new CombatGameState(
                                _manager,
                                new List<Enemy> { encountered },
                                _loader));
                            return;
                        }
                    }

                    // Display new room - OnEnter called once here
                    _manager.Renderer.ClearScreen();
                    var newRoom = _manager.Map.GetCurrentRoom();
                    var desc = newRoom.OnEnter(_manager.Player);
                    var roomDescription = _loader.GetRoomDescription(newRoom.GetRoomID());
                    _manager.Renderer.RenderMessage(desc);
                    
                    _manager.Renderer.RenderRoom(newRoom, _manager.Player, roomDescription);
                    continue;
                }
                
                choice -= neighbours.Count;
                
                // Attack
                if (enemies.Count > 0 && choice == 0)
                {
                    _manager.ChangeState(new CombatGameState(
                        _manager, enemies, _loader));
                    return;
                }
                
                if (enemies.Count > 0) choice--;
                
                // Handle interactions
                if (choice < interactables.Count)
                {
                    var interactable = interactables[choice];
                    
                    // start dialogue
                    if (interactable is IDialoguable dialoguable)
                    {
                        HandleNPCDialogue(dialoguable);
                        continue;
                    }
                    
                    // open shop
                    if (interactable is ShopkeeperNPC shopkeeper)
                    {
                        HandleShop(shopkeeper);
                        continue;
                    }
                    
                    // Everything else - display returned message
                    var result = interactable.Interact(_manager.Player);
                    _manager.Renderer.RenderInteractionResult(result);
                    if (!string.IsNullOrEmpty(result))
                    {
                        _manager.Renderer.RenderMessage("Press Enter, to continue...");
                        Console.ReadLine();
                    }
                    continue;
                }
                
                choice -= interactables.Count;
                
                // handle always available options
                switch (choice)
                {
                    case 0:
                        HandleInventory();
                        break;
                    case 1:
                        var saveMsg = new SaveManager().SaveGame(_manager.Player, _manager.Map);
                        _manager.Renderer.RenderMessage(saveMsg);
                        _manager.Renderer.RenderMessage("Press Enter, to continue...");
                        Console.ReadLine();
                        break;
                    case 2:
                        _manager.Quit();
                        return;
                }
            }
        }

        private void HandleInventory()
        {
            while (true)
            {
                _manager.Renderer.RenderInventory(_manager.Player);
                _manager.Renderer.RenderMenu("Inventory options:", new List<string>
                {
                    "Equip/Unequip item",
                    "Use consumable",
                    "Back"
                });
                
                if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

                switch (choice)
                {
                    case 1:
                        HandleEquip();
                        break;
                    case 2:
                        HandleUseConsumable();
                        break;
                    case 3:
                        return;
                }
            }
        }

        private void HandleEquip()
        {
            _manager.Renderer.RenderEquipScreen(_manager.Player);
            
            var equippables = _manager.Player.PlayerInventory.GetEquippables();
            if (equippables.Count == 0) return;
            
            _manager.Renderer.RenderMenu("Equip which item? (0 to cancel):",
                equippables.Select(e => $"{e.Name} [{e.Slot}]").ToList());
            
            if (!int.TryParse(Console.ReadLine(), out int choice) ||
                choice == 0 || choice > equippables.Count) return;
            
            var item = equippables[choice - 1];
            
            // Check if slot already occupied
            var current = _manager.Player.Equipment.GetSlot(item.Slot);
            if (current != null)
            {
                _manager.Renderer.RenderMessage(
                    $"Unequipped: {current.Name}");
                _manager.Player.Equipment.Unequip(item.Slot, _manager.Player);
            }
            
            _manager.Player.Equipment.Equip(item, _manager.Player);
            _manager.Player.PlayerInventory.RemoveItem(item);
            _manager.Renderer.RenderMessage($"Equipped: {item.Name}");
        }

        private void HandleUseConsumable()
        {
            var consumables = _manager.Player.PlayerInventory.GetConsumables();
            if (consumables.Count == 0)
            {
                _manager.Renderer.RenderMessage("No consumables.");
                return;
            }
            _manager.Renderer.RenderMenu("Use which item?",
                consumables.Select(c => c.Name).ToList());
            
            if (!int.TryParse(Console.ReadLine(), out int choice) ||
                choice < 1 || choice > consumables.Count) return;
            
            var item = consumables[choice - 1];
            int hpBefore = _manager.Player.CurrentHP;
            bool consumed = item.Use(_manager.Player);
            int healed = _manager.Player.CurrentHP - hpBefore;
            
            if (healed > 0)
                _manager.Renderer.RenderMessage(
                    $"Used {item.Name}. Restored {healed} HP.");
            else
                _manager.Renderer.RenderMessage($"Used {item.Name}.");
            
            if (consumed)
                _manager.Player.PlayerInventory.RemoveItem(item);
        }

        private void HandleNPCDialogue(IDialoguable dialoguable)
        {
            _dialogueEngine.StartConversation(dialoguable, _manager.Player);

            while (_dialogueEngine.IsActive)
            {
                var node = _dialogueEngine.CurrentNode;
                if (node == null) break;
                
                _manager.Renderer.RenderDialogue(node);
                
                if (node.Choices.Count == 0) 
                {
                    _manager.Renderer.RenderMessage("Press Enter, to continue...");
                    Console.ReadLine();
                    break;
                }
                
                if (!int.TryParse(Console.ReadLine(), out int choice) ||
                    choice < 1 || choice > node.Choices.Count)
                {
                    _manager.Renderer.RenderMessage("Invalid choice.");
                    continue;
                }
                
                var msg = _dialogueEngine.SelectChoice(choice - 1, _manager.Player);
                
                if (!string.IsNullOrEmpty(msg))
                {
                    _manager.Renderer.RenderInteractionResult(msg);
                    _manager.Renderer.RenderMessage("Press Enter, to continue..."); // DODANE
                    Console.ReadLine();
                }
            }
            _dialogueEngine.EndConversation();
        }

        private void HandleShop(ShopkeeperNPC shopkeeper)
        {
            _manager.Renderer.RenderMessage(
                $"{shopkeeper.Name}: Welcome! What are you buying?");

            var shop = shopkeeper.GetShop();

            while (true)
            {
                var stock = shop.GetStock();
                
                _manager.Renderer.RenderShop(stock, _manager.Player);
                
                var options = stock
                    .Select(s => $"{s.item.Name} - {s.price} coins")
                    .ToList();
                options.Add("Sell an item");
                options.Add("Leave");
                
                // render shop is showing the menu
                Console.Write("\nChoice: ");
                
                if (!int.TryParse(Console.ReadLine(), out int choice) ||
                    choice < 1 || choice > options.Count)
                {
                    _manager.Renderer.RenderMessage("Invalid choice.");
                    continue;
                }
                
                choice--;
                
                if (choice == options.Count - 1) break; // Leave

                if (choice == options.Count - 2) // Sell
                {
                    HandleSellToShop(shop);
                    continue;
                }
                
                var (item, message) = shop.Sell(choice, _manager.Player);
                _manager.Renderer.RenderMessage(message);
                
                if (item != null)
                    _manager.Player.PlayerInventory.AddItem(item);
            }
        }

        private void HandleSellToShop(Shop.IShop shop)
        {
            var consumables = _manager.Player.PlayerInventory
                .GetConsumables().Cast<Inventory.Interfaces.IItem>().ToList();
            
            var equippables = _manager.Player.PlayerInventory
                .GetEquippables().Cast<Inventory.Interfaces.IItem>().ToList();
            
            var sellable = consumables.Concat(equippables).ToList();
            
            if (sellable.Count == 0)
            {
                _manager.Renderer.RenderMessage("Nothing to sell.");
                return;
            }
            
            var options = sellable.Select(i => i.Name).ToList();
            _manager.Renderer.RenderMenu("Sell which item?", options);
            
            if (!int.TryParse(Console.ReadLine(), out int choice) ||
                choice < 1 || choice > sellable.Count) return;
            
            var msg = shop.BuyFromPlayer(
                sellable[choice - 1], _manager.Player);
            _manager.Renderer.RenderMessage(msg);
        }
    }
}