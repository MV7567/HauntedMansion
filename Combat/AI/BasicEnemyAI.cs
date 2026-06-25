using HauntedMansion.Combat.Actions;
using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Entities;

namespace HauntedMansion.Combat.AI
{
    /// <summary>
    /// Simple AI that always attacks the player.
    /// Used as default for all enemies until more complex AI is needed.
    /// </summary>
    public class BasicEnemyAI : IEnemyAi
    {
        // engine to pass into AttackActions
        private readonly CombatEngine _engine = new();
        private readonly Random _rng = new();
        
        // creates an appropriate IAction for the turn, based on the situation
        public IAction ChooseAction(Enemy enemy, CombatContext context)
        {
            // simple version: always attacks, 50/50 magic or physical
            var type = _rng.NextDouble() < 0.5 
                ? AttackType.Physical 
                : AttackType.Magical;
            
            // Returns the command object to CombatGameState
            // Execute() when right turn
            return new AttackAction(enemy, _engine, type);
        }
    }
}