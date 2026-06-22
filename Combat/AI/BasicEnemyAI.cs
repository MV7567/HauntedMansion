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
        private readonly CombatEngine _engine = new();
        private readonly Random _rng = new();
        
        public IAction ChooseAction(Enemy enemy, CombatContext context)
        {
            var type = _rng.NextDouble() < 0.5 
                ? AttackType.Physical 
                : AttackType.Magical;
            return new AttackAction(enemy, _engine, type);
        }
    }
}