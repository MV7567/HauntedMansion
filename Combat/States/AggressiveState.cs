using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Entities;

namespace HauntedMansion.Combat.States
{
    /// <summary>
    /// Enemy is provoked... (future implementation)
    /// </summary>
    public class AggressiveState : ICombatState
    {
        public void OnEnter(Enemy enemy)
        {
            // Triggered when entering this state
            // Future: Apply a temporary StatModifier to increase attack
        }

        public IAction Execute(Enemy enemy, CombatContext context)
        {
            // Instead of using AI a specific aggressive action is chosen
            return null;
        }

        public void OnExit(Enemy enemy)
        {
            // Triggered when leaving the state
            // Future: Remove the stat modifier applied on enter
        }
    }
}