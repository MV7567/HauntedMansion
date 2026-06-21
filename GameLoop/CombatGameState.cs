using HauntedMansion.Combat;
using HauntedMansion.Combat.States;
using HauntedMansion.Data;
using HauntedMansion.Dialogue;
using HauntedMansion.Entities;
using HauntedMansion.UI;
using HauntedMansion.UI.Commands;

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
        private readonly CombatEngine _combatEngine = new();
        private readonly CombatContext _context;

        public CombatGameState(GameManager manager, List<Enemy> enemies)
        {
            _manager = manager;
            _context = new CombatContext
            {
                Player = manager.Player,
                Enemies = enemies,
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
                    _manager.ChangeState(new ExplorationGameState(_manager));
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
            
            // Select body part
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
                _manager.Renderer.RenderMessage("Invalid body part.");
                return false;
            }

            var result = _combatEngine.ExecuteAttack(_manager.Player, target, type, part);
            
            _manager.Renderer.RenderMessage(result.Message);
            return true;
        }

        private bool HandleItem()
        {
            var consumables = _manager.Player.PlayerInventory.GetConsumables();
            if (consumables.Count == 0)
            {
                _manager.Renderer.RenderMessage("No comsumables.");
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
            var dialogueable = enemies.OfType<IDialoguable>().FirstOrDefault();

            if (dialogueable == null)
            {
                _manager.Renderer.RenderMessage("This enemy cannot be reasoned with.");
                return false;
            }
            
            // future: full dialogue engine integration
            _manager.Renderer.RenderMessage("You attempt to talk to the enemy...");
            return true;
        }

        private bool HandleSpare(List<Enemy> enemies)
        {
            var sparable = enemies.OfType<NormalEnemy>().FirstOrDefault(e => e.IsAlive());

            if (sparable == null)
            {
                _manager.Renderer.RenderMessage("No enemy can be spared.");
                return false;
            }
            
            // future: check if the enemy is in SparableState
            sparable.MarkDefeated();
            _manager.Renderer.RenderMessage($"{sparable.Name} has been spared.");
            _context.Enemies.Remove(sparable);
            return true;
        }

        private void HandleEnemyTurn(Enemy enemy)
        {
            // future: use enemy AI and State
            // placeholder: simple attack
            var result = _combatEngine.ExecuteAttack(
                enemy, _manager.Player, AttackType.Physical);
            _manager.Renderer.RenderMessage(result.Message);
        }
    }
}