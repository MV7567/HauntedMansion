using HauntedMansion.Combat;
using HauntedMansion.Combat.Actions;
using HauntedMansion.Data;
using HauntedMansion.Dialogue;
using HauntedMansion.Entities;
using HauntedMansion.UI;

namespace HauntedMansion.GameLoop
{
    /// <summary>
    /// turn based combat
    /// delegates calcs to CombatEngine
    /// checks conditions each turn
    /// </summary>
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
            _context = new CombatContext(manager.Player, enemies)
            {
                TurnNumber = 1
            };
        }

        public void OnEnter()
        {
            _manager.Renderer.RenderMessage("Combat begins!");
            RunCombatLoop();
        }

        public void OnExit() { }

        public void HandleInput(ICommand command)
        {
            command.Execute(_context);
        }

        /// <summary>
        /// main combat loop, run until all enemies (or player) defeated
        /// </summary>
        private void RunCombatLoop()
        {
            while (true)
            {
                _manager.Renderer.RenderCombat(_context);
                
                // Check win condition
                if (_context.Enemies.All(e => !e.IsAlive()))
                {
                    _manager.Renderer.RenderMessage("All enemies defeated!");
                    _manager.ChangeState(new ExplorationGameState(_manager, _loader));
                    return;
                }
                
                // Check lose condition
                if (!_manager.Player.IsAlive())
                {
                    _manager.Renderer.RenderMessage("You died!");
                    _manager.Quit();
                    return;
                }
                
                // Player turn
                bool validAction = false;
                while (!validAction)
                    validAction = HandlePlayerTurn();
                
                // Enemy turns
                foreach (var enemy in _context.Enemies.Where(e => e.IsAlive()))
                    HandleEnemyTurn(enemy);

                _context.TurnNumber++;
            }
        }

        /// <summary>
        /// combat menu and handles player choice
        /// </summary>
        private bool HandlePlayerTurn()
        {
            var enemies = _context.Enemies.Where(e => e.IsAlive()).ToList();
            
            _manager.Renderer.RenderMenu("Your turn:", new List<string>
            {
                "Attack",
                "Magic",
                "Use item",
                "Talk",
                "Spare"
            });

            if (!int.TryParse(Console.ReadLine(), out int choice))
                return false;

            switch (choice)
            {
                case 1: return HandleAttack(AttackType.Physical, enemies);
                case 2: return HandleAttack(AttackType.Magical, enemies);
                case 3: return HandleItem();
                case 4: return HandleDialogue(enemies);
                case 5: return HandleSpare(enemies);
                default: return false;
            }
        }

        private bool HandleAttack(AttackType type, List<Enemy> enemies)
        {
            // Select enemy
            var enemyOptions = enemies.Select(e => e.Name).ToList();
            _manager.Renderer.RenderMenu("Target enemy:", enemyOptions);

            if (!int.TryParse(Console.ReadLine(), out int eChoice) ||
                eChoice < 1 || eChoice > enemies.Count) return false;

            var target = enemies[eChoice - 1];

            var partOptions = Enum.GetValues(typeof(BodyPartType))
                .Cast<BodyPartType>()
                .Select(p => p.ToString())
                .ToList();

            _manager.Renderer.RenderMenu("Target body part:", partOptions);

            if (!int.TryParse(Console.ReadLine(), out int pChoice) ||
                pChoice < 1 || pChoice > partOptions.Count) return false;

            var partType = (BodyPartType)(pChoice - 1);
            var part = target.GetBodyPart(partType);

            if (part == null)
            {
                _manager.Renderer.RenderError("Invalid body part.");
                return false;
            }

            var result = _combatEngine.ExecuteAttack(
                _manager.Player, target, type, part);
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
            _manager.Renderer.RenderMenu("Use item:", options);

            if (!int.TryParse(Console.ReadLine(), out int choice) ||
                choice < 1 || choice > consumables.Count) return false;
            
            var item = consumables[choice - 1];
            bool consumed = item.Use(_manager.Player);
            
            if (consumed)
                _manager.Player.PlayerInventory.RemoveItem(item);

            return true;
        }

        private bool HandleDialogue(List<Enemy> enemies)
        {
            // only enemies with dialogue
            var dialoguable = enemies.OfType<IDialoguable>().FirstOrDefault();

            if (dialoguable == null)
            {
                _manager.Renderer.RenderMessage("This enemy cannot be reasoned with.");
                return false;
            }
            
            // Start dialogue - DialogueEngine handles navigation
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
                    choice - 1, _manager.Player, _context);
                
                if (!string.IsNullOrEmpty(msg))
                    _manager.Renderer.RenderStateChange(msg);
            }
            _dialogueEngine.EndConversation();
            return true;
        }

        private bool HandleSpare(List<Enemy> enemies)
        {
            var sparable = enemies
                .Where(e => e.IsAlive())
                .FirstOrDefault(e => e.IsSparable(_context));

            if (sparable == null)
            {
                _manager.Renderer.RenderMessage(
                    "No enemy can be spared.");
                return false;
            }

            if (sparable is NormalEnemy normal)
                normal.MarkDefeated();
            else if (sparable is BossEnemy boss)
                boss.BecomeNPC();

            _manager.Renderer.RenderStateChange(
                $"{sparable.Name} has been spared.");
            _context.Enemies.Remove(sparable);
            return true;
        }

        private void HandleEnemyTurn(Enemy enemy)
        {
            var action = enemy.GetAction(_context);
            if (action == null) return;

            var result = action.Execute(_context);
            _manager.Renderer.RenderCombatResult(result);
        }
    }
}