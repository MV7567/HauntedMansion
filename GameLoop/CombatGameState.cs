using HauntedMansion.Combat;
using HauntedMansion.Data;
using HauntedMansion.Dialogue;
using HauntedMansion.Entities;
using HauntedMansion.UI;

namespace HauntedMansion.GameLoop
{
    public class CombatGameState : IGameState
    {
        private readonly GameManager _manager;
        private readonly IContentLoader _loader;
        private readonly CombatEngine _combatEngine = new();
        private readonly CombatContext _context;
        private readonly DialogueEngine _dialogueEngine;

        public CombatGameState(GameManager manager, List<Enemy> enemies, IContentLoader loader)
        {
            _manager = manager;
            _loader = loader;
            _dialogueEngine = new DialogueEngine(loader);
            _context = new CombatContext(manager.Player, enemies) { TurnNumber = 1 };
        }

        public void OnEnter()
        {
            _manager.Renderer.ClearScreen();
            _manager.Renderer.RenderMessage("Combat begins!");
            _manager.WaitToContinue();
        }

        public void OnExit() { }

        public void Update()
        {
            _manager.Renderer.ClearScreen();
            _manager.Renderer.RenderCombat(_context);
            
            if (_context.Enemies.All(e => !e.IsAlive()))
            {
                _manager.Renderer.RenderMessage("All enemies defeated!");
                _manager.WaitToContinue();
                _manager.ChangeState(new ExplorationGameState(_manager, _loader));
                return;
            }
            
            if (!_manager.Player.IsAlive())
            {
                _manager.Renderer.RenderMessage("You died!");
                _manager.WaitToContinue(); 
                _manager.Quit();
                return;
            }
            
            bool validAction = false;
            while (!validAction) validAction = HandlePlayerTurn();
            
            foreach (var enemy in _context.Enemies.Where(e => e.IsAlive())) HandleEnemyTurn(enemy);

            _context.TurnNumber++;
            _manager.WaitToContinue(); 
        }

        private bool HandlePlayerTurn()
        {
            var enemies = _context.Enemies.Where(e => e.IsAlive()).ToList();
            var options = new List<string> { "Attack", "Magic", "Use item", "Talk", "Spare" };
            
            _manager.Renderer.RenderMenu("Your turn:", options);
            int choice = _manager.Input.GetIntInput(1, options.Count);

            bool success = choice switch
            {
                1 => HandleAttack(AttackType.Physical, enemies),
                2 => HandleAttack(AttackType.Magical, enemies),
                3 => HandleItem(),
                4 => HandleDialogue(enemies),
                5 => HandleSpare(enemies),
                _ => false
            };

            if (!success) _manager.WaitToContinue(); 
            return success;
        }

        private bool HandleAttack(AttackType type, List<Enemy> enemies)
        {
            var enemyOptions = enemies.Select(e => e.Name).ToList();
            enemyOptions.Add("Cancel");
            
            _manager.Renderer.RenderMenu("Target enemy:", enemyOptions);
            int eChoice = _manager.Input.GetIntInput(1, enemyOptions.Count);
            if (eChoice == enemyOptions.Count) return false;

            var target = enemies[eChoice - 1];
            var availableParts = Enum.GetValues(typeof(BodyPartType))
                .Cast<BodyPartType>()
                .Select(p => new { Type = p, Part = target.GetBodyPart(p) })
                .Where(x => x.Part != null)
                .ToList();

            var partOptions = availableParts.Select(x => x.Part!.IsDisabled ? $"{x.Type} [DISABLED]" : x.Type.ToString()).ToList();
            partOptions.Add("Cancel");

            _manager.Renderer.RenderMenu("Target body part:", partOptions);
            int pChoice = _manager.Input.GetIntInput(1, partOptions.Count);
            if (pChoice == partOptions.Count) return false;

            var selectedPart = availableParts[pChoice - 1].Part!;
            var result = _combatEngine.ExecuteAttack(_manager.Player, target, type, selectedPart);
            _manager.Renderer.RenderCombatResult(result);
            return true;
        }

        private bool HandleItem()
        {
            var consumables = _manager.Player.PlayerInventory.GetConsumables();
            if (consumables.Count == 0)
            {
                _manager.Renderer.RenderMessage("You don't have any consumables.");
                return false;
            }

            var options = consumables.Select(c => c.Name).ToList();
            options.Add("Cancel");
            
            _manager.Renderer.RenderMenu("Use item:", options);
            int choice = _manager.Input.GetIntInput(1, options.Count);
            if (choice == options.Count) return false;
            
            var item = consumables[choice - 1];
            if (item.Use(_manager.Player)) _manager.Player.PlayerInventory.RemoveItem(item);
            return true;
        }

        private bool HandleDialogue(List<Enemy> enemies)
        {
            var dialoguable = enemies.OfType<IDialoguable>().FirstOrDefault();
            if (dialoguable == null)
            {
                _manager.Renderer.RenderMessage("This enemy cannot be reasoned with.");
                return false;
            }
            
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
                var msg = _dialogueEngine.SelectChoice(choice - 1, _manager.Player, _context);
                if (!string.IsNullOrEmpty(msg)) _manager.Renderer.RenderStateChange(msg);
            }
            _dialogueEngine.EndConversation();
            return true;
        }

        private bool HandleSpare(List<Enemy> enemies)
        {
            var sparable = enemies.Where(e => e.IsAlive()).FirstOrDefault(e => e.IsSparable);

            if (sparable == null)
            {
                _manager.Renderer.RenderMessage("No enemy can be spared right now.");
                return false;
            }

            if (sparable is NormalEnemy normal) normal.MarkDefeated();
            else if (sparable is BossEnemy boss) boss.BecomeNPC();

            _manager.Renderer.RenderStateChange($"{sparable.Name} has been spared.");
            _context.Enemies.Remove(sparable);
            return true;
        }

        private void HandleEnemyTurn(Enemy enemy)
        {
            var action = enemy.GetAction(_context);
            if (action != null)
            {
                var result = action.Execute(_context);
                _manager.Renderer.RenderCombatResult(result);
            }
        }
    }
}