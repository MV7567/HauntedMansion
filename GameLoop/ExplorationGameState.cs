using HauntedMansion.Data;
using HauntedMansion.Dialogue;
using HauntedMansion.Entities;
using HauntedMansion.Interactions;
using HauntedMansion.UI;
using HauntedMansion.World;

namespace HauntedMansion.GameLoop
{
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

        public void OnEnter() { }
        public void OnExit() { }

        public void Update()
        {
            _manager.Renderer.ClearScreen();
            var room = _manager.Map.GetCurrentRoom();
            var descr = _loader.GetRoomDescription(room.GetRoomID());
            
            _manager.Renderer.RenderRoom(room, _manager.Player, descr);
            
            var neighbours = _manager.Map.GetNeighbours(room.GetRoomID());
            var interactables = room.GetInteractables();
            var enemies = room.GetEnemies();

            var options = new List<string>();
            var actions = new List<Action>();

            foreach (var neighbour in neighbours)
            {
                var target = neighbour;
                options.Add($"Go to {target.GetRoomID().Replace('_', ' ')}");
                actions.Add(() => HandleMovement(target));
            }
            
            if (enemies.Count > 0)
            {
                options.Add($"Attack! ({enemies.Count} {(enemies.Count == 1 ? "enemy" : "enemies")} present)");
                actions.Add(() => _manager.ChangeState(new CombatGameState(_manager, enemies, _loader)));
            }
            
            foreach (var obj in interactables)
            {
                var interactable = obj;
                options.Add($"Examine: {interactable.GetDescription()}");
                actions.Add(() => HandleInteraction(interactable));
            }
            
            options.Add("Open inventory");
            actions.Add(HandleInventory);
            
            options.Add("Save game");
            actions.Add(() => {
                var msg = new UI.SaveManager().SaveGame(_manager.Player, _manager.Map);
                _manager.Renderer.RenderMessage(msg);
                _manager.WaitToContinue();
            });
            
            options.Add("Quit");
            actions.Add(_manager.Quit);
            
            _manager.Renderer.RenderMenu("What do you do?", options);
            
            int choice = _manager.Input.GetIntInput(1, options.Count);
            actions[choice - 1].Invoke();
        }

        private void HandleMovement(IRoom target)
        {
            var (success, msg) = _manager.Map.MoveToRoom(target.GetRoomID(), _manager.Player);
            if (!success)
            {
                _manager.Renderer.RenderMessage(msg);
                _manager.WaitToContinue();
                return;
            }

            var newRoom = _manager.Map.GetCurrentRoom();
            
            if (newRoom is Room concreteRoom)
            {
                var encountered = concreteRoom.TryTriggerEncounter(_manager.Player);
                if (encountered != null)
                {
                    _manager.Renderer.RenderMessage($"\nA {encountered.Name} appears!");
                    _manager.WaitToContinue();
                    _manager.ChangeState(new CombatGameState(_manager, new List<Enemy> { encountered }, _loader));
                    return;
                }
            }

            _manager.Renderer.ClearScreen();
            var desc = newRoom.OnEnter(_manager.Player);
            _manager.Renderer.RenderMessage(desc);
            _manager.WaitToContinue();
        }

        private void HandleInteraction(IInteractable interactable)
        {
            if (interactable is IDialoguable dialoguable)
                HandleNPCDialogue(dialoguable);
            else if (interactable is ShopkeeperNPC shopkeeper)
                HandleShop(shopkeeper);
            else
            {
                var result = interactable.Interact(_manager.Player);
                _manager.Renderer.RenderInteractionResult(result);
                if (!string.IsNullOrEmpty(result)) _manager.WaitToContinue();
            }
        }

        private void HandleInventory()
        {
            while (true)
            {
                _manager.Renderer.ClearScreen();
                
                _manager.Renderer.RenderInventory(_manager.Player);
                var options = new List<string> { "Equip/Unequip item", "Use consumable", "Back" };
                _manager.Renderer.RenderMenu("Inventory options:", options);
                
                int choice = _manager.Input.GetIntInput(1, options.Count);
                if (choice == 1) HandleEquip();
                else if (choice == 2) HandleUseConsumable();
                else return;
            }
        }

        private void HandleEquip()
        {
            _manager.Renderer.ClearScreen();
            
            _manager.Renderer.RenderEquipScreen(_manager.Player);
            var equippables = _manager.Player.PlayerInventory.GetEquippables();
            if (equippables.Count == 0)
            {
                _manager.WaitToContinue();
                return;
            }
            
            var options = equippables.Select(e => $"{e.Name} [{e.Slot}]").ToList();
            options.Add("Cancel");
            
            _manager.Renderer.RenderMenu("Equip which item?", options);
            int choice = _manager.Input.GetIntInput(1, options.Count);
            if (choice == options.Count) return;
            
            var item = equippables[choice - 1];
            var current = _manager.Player.Equipment.GetSlot(item.Slot);
            
            if (current != null)
            {
                _manager.Renderer.RenderMessage($"Unequipped: {current.Name}");
                _manager.Player.Equipment.Unequip(item.Slot, _manager.Player);
            }
            
            _manager.Player.Equipment.Equip(item, _manager.Player);
            _manager.Player.PlayerInventory.RemoveItem(item);
            _manager.Renderer.RenderMessage($"Equipped: {item.Name}");
            _manager.WaitToContinue();
        }

        private void HandleUseConsumable()
        {
            _manager.Renderer.ClearScreen();
            
            var consumables = _manager.Player.PlayerInventory.GetConsumables();
            if (consumables.Count == 0)
            {
                _manager.Renderer.RenderMessage("No consumables.");
                _manager.WaitToContinue();
                return;
            }
            
            var options = consumables.Select(c => c.Name).ToList();
            options.Add("Cancel");
            
            _manager.Renderer.RenderMenu("Use which item?", options);
            int choice = _manager.Input.GetIntInput(1, options.Count);
            if (choice == options.Count) return;
            
            var item = consumables[choice - 1];
            int hpBefore = _manager.Player.CurrentHP;
            bool consumed = item.Use(_manager.Player);
            int healed = _manager.Player.CurrentHP - hpBefore;
            
            _manager.Renderer.RenderMessage(healed > 0 ? $"Used {item.Name}. Restored {healed} HP." : $"Used {item.Name}.");
            if (consumed) _manager.Player.PlayerInventory.RemoveItem(item);
            _manager.WaitToContinue();
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
                    _manager.WaitToContinue();
                    break;
                }
                
                int choice = _manager.Input.GetIntInput(1, node.Choices.Count);
                var msg = _dialogueEngine.SelectChoice(choice - 1, _manager.Player);
                
                if (!string.IsNullOrEmpty(msg))
                {
                    _manager.Renderer.RenderInteractionResult(msg);
                    _manager.WaitToContinue();
                }
            }
            _dialogueEngine.EndConversation();
        }

        private void HandleShop(ShopkeeperNPC shopkeeper)
        {
            // Retrieve the shop instance from the NPC
            var shop = shopkeeper.GetShop();

            while (true)
            {
                _manager.Renderer.ClearScreen();
                _manager.Renderer.RenderMessage($"{shopkeeper.Name}: Welcome! What are you buying?");
                
                var stock = shop.GetStock();
                _manager.Renderer.RenderShop(stock, _manager.Player);
                
                int maxChoice = stock.Count + 2;
                int choice = _manager.Input.GetIntInput(1, maxChoice);
                
                if (choice == maxChoice) break;
                if (choice == maxChoice - 1)
                {
                    HandleSellToShop(shop);
                    continue;
                }
                
                var (item, message) = shop.Sell(choice - 1, _manager.Player);
                _manager.Renderer.RenderMessage(message);
                if (item != null) _manager.Player.PlayerInventory.AddItem(item);
                _manager.WaitToContinue();
            }
        }

        private void HandleSellToShop(Shop.IShop shop)
        {
            _manager.Renderer.ClearScreen();
            
            var sellable = _manager.Player.PlayerInventory.GetConsumables().Cast<Inventory.Interfaces.IItem>()
                .Concat(_manager.Player.PlayerInventory.GetEquippables()).ToList();
            
            if (sellable.Count == 0)
            {
                _manager.Renderer.RenderMessage("Nothing to sell.");
                _manager.WaitToContinue();
                return;
            }
            
            var options = sellable.Select(i => i.Name).ToList();
            options.Add("Cancel");
            
            _manager.Renderer.RenderMenu("Sell which item?", options);
            int choice = _manager.Input.GetIntInput(1, options.Count);
            if (choice == options.Count) return;
            
            var msg = shop.BuyFromPlayer(sellable[choice - 1], _manager.Player);
            _manager.Renderer.RenderMessage(msg);
            _manager.WaitToContinue();
        }
    }
}