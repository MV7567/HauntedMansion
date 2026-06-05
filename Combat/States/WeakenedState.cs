using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Entities;

namespace HauntedMansion.Combat.States
{
    /// <summary>
    /// Enemy is at low HP and/or missing body parts
    /// </summary>
    public class WeakenedState : ICombatState
    {
        public void OnEnter(Enemy enemy) {}

        public IAction Execute(Enemy enemy, CombatContext context)
        {
            // Force defensive behavior or chance to flee
            // Future: return new DefendAction() or FleeAction();
            return null;
        }
        
        public void OnExit(Enemy enemy) {}
    }
}