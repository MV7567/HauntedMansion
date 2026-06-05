using HauntedMansion.Entities;

namespace HauntedMansion.Combat.Interfaces
{
    /// <summary>
    /// For enemy decision making
    /// swapping enemy logic without changing the enemy class
    /// </summary>
    public interface IEnemyAi
    {
        IAction ChooseAction(Enemy enemy, CombatContext context);
    }
}