using HauntedMansion.Entities;

namespace HauntedMansion.Combat.Interfaces
{
    /// <summary>
    /// For enemy decision making
    /// strategy
    /// swapping enemy logic without changing the enemy class
    /// </summary>
    public interface IEnemyAi
    {
        //Analyzes the battlefield context and chooses an action for the turn
        IAction ChooseAction(Enemy enemy, CombatContext context);
    }
}