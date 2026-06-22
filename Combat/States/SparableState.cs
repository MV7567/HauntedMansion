using HauntedMansion.Combat.Actions;
using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Entities;

namespace HauntedMansion.Combat.States
{
    // Enemy has been appeased through dialogue or items
    public class SparableState : ICombatState
    {
        public string OnEnter(Enemy enemy)
        {
            return $"{enemy.Name} calms down. You can spare them.";
        }

        public IAction Execute(Enemy enemy, CombatContext context)
        {
            // Enemy does nothing on its turn
            return new IdleAction($"{enemy.Name} doesn't attack.");
        }
        
        public string OnExit(Enemy enemy) => string.Empty;
    }
}