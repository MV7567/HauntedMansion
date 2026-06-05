using HauntedMansion.Entities;

namespace HauntedMansion.Combat.Interfaces
{
    /// <summary>
    /// Controls enemy behavior based on their current condition
    /// </summary>
    public interface ICombatState
    {
        IAction Execute(Enemy enemy, CombatContext context);
        void OnEnter(Enemy enemy);
        void OnExit(Enemy enemy);
    }
}