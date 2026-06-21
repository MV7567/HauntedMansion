using HauntedMansion.Combat.Actions;
using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Entities;

namespace HauntedMansion.Combat.States
{
    /// <summary>
    /// Enemy is provoked - higher attack, always attacks, lower accuracy
    /// </summary>
    public class AggressiveState : ICombatState
    {
        private const int AttackBonus = 5;
        private const int AccuracyDrop = -15;
        private readonly CombatEngine _engine = new();
        
        public void OnEnter(Enemy enemy)
        {
            enemy.ApplyStateMod(attack: AttackBonus, accuracy: AccuracyDrop);
            Console.WriteLine($"{enemy.Name} becomes aggressive!");
        }

        public IAction Execute(Enemy enemy, CombatContext context)
        {
            // Always attacks - ignores AI
            return new AttackAction(enemy, _engine, AttackType.Physical);
        }

        public void OnExit(Enemy enemy)
        {
            enemy.ResetStateMod();
        }
    }
}