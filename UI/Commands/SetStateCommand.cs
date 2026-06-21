using HauntedMansion.Combat;
using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Entities;

namespace HauntedMansion.UI.Commands
{
    /// <summary>
    /// changes enemy combat state
    /// used by dialogue choice to trigger state transition
    /// </summary>
    public class SetStateCommand : ICommand
    {
        private readonly Enemy _enemy;
        private readonly ICombatState _newState;

        public SetStateCommand(Enemy enemy, ICombatState newState)
        {
            _enemy = enemy;
            _newState = newState;
        }

        public void Execute(CombatContext context = null)
        {
            _enemy.SetState(_newState);
        }
    }
}