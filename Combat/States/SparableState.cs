using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Entities;

namespace HauntedMansion.Combat.States
{
    // Enemy has been appeased through dialogue or items
    public class SparableState : ICombatState
    {
        public void OnEnter(Enemy enemy)
        {
            // Signals to the game loop that this enemy can now be spared
        }

        public IAction Execute(Enemy enemy, CombatContext context)
        {
            // A spared enemy does nothing on its turn 
            // Future: return new IdleAction();
            return null;
        }
        
        public void OnExit(Enemy enemy) {}
    }
}