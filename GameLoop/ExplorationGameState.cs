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
            var description = _manager.Map.GetCurrentRoom().OnEnter(_manager.Player);
            _manager.Renderer.RenderMessage(description);
            _manager.Renderer.RenderRoom(_manager.Map.GetCurrentRoom(), _manager.Player);
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
                var room = _manager.Map.GetCurrentRoom();
                var neighbours = _manager.Map.GetNeighbours(room.GetRoomID());
                var interactables = room.GetInteractables();

                var options = new List<string>();
                
                // movment options
                foreach (var neighbour in neighbours)
                    options.Add($"Go to {neighbour.GetRoomID().Replace('_', ' ')}");
                
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
                    
                    // check for random encounter after moving
                    if (room is Room concreteRoom)
                    {
                        var encountered = concreteRoom
                            .TryTriggerEncounter(_manager.Player);

                        if (encountered != null)
                        {
                            _manager.Renderer.RenderMessage($"A {encountered.Name} appears!");
                            _manager.ChangeState(new CombatGameState(
                                _manager,new List<Enemy> { encountered }, 
                                _loader));
                            return;
                        }
                    }
                    
                    // Display new room
                    var desc = _manager.Map.GetCurrentRoom().OnEnter(_manager.Player);
                    _manager.Renderer.RenderMessage(desc);
                    _manager.Renderer.RenderRoom(_manager.Map.GetCurrentRoom(), _manager.Player);
                    continue;
                }
                
                choice -= neighbours.Count;
                
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
                    continue;
                }
                
                choice -= interactables.Count;
                
                // handle always available options
                switch (choice)
                {
                    case 0:
                        _manager.Renderer.RenderInventory(_manager.Player);
                        break;
                    case 1:
                        var saveMsg = new SaveManager().SaveGame(_manager.Player, _manager.Map);
                        _manager.Renderer.RenderMessage(saveMsg);
                        break;
                    case 2:
                        _manager.Quit();
                        return;
                }
            }
        }

        private void HandleNPCDialogue(IDialoguable dialoguable)
        {
            _dialogueEngine.StartConversation(dialoguable);

            while (_dialogueEngine.IsActive)
            {
                var node = _dialogueEngine.CurrentNode;
                if (node == null) break;
                
                _manager.Renderer.RenderDialogue(node);
                
                if (node.Choices.Count == 0) break;
                
                if (!int.TryParse(Console.ReadLine(), out int choice) ||
                    choice < 1 || choice > node.Choices.Count)
                {
                    _manager.Renderer.RenderMessage("Invalid choice.");
                    continue;
                }
                
                var msg = _dialogueEngine.SelectChoice(
                    choice - 1, _manager.Player);
                
                if (!string.IsNullOrEmpty(msg))
                    _manager.Renderer.RenderInteractionResult(msg);
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
                
                var options = stock.Select((s, i) => $"{s.item.Name} - {s.price} coins").ToList();
                options.Add("Sell an item");
                options.Add("Leave");
                
                _manager.Renderer.RenderMenu("Shop:", options);
                
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