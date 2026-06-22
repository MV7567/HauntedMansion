using HauntedMansion.Entities;

namespace HauntedMansion.Combat.Interfaces
{
    /// <summary>
    /// Controls enemy behavior based on their current condition
    /// </summary>
    public interface ICombatState
    {
        IAction Execute(Enemy enemy, CombatContext context);
        string OnEnter(Enemy enemy);  // returns message for renderer
        string OnExit(Enemy enemy);   // returns message for renderer
    }
}