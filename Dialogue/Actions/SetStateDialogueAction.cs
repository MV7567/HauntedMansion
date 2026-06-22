using HauntedMansion.Combat;
using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Entities;

namespace HauntedMansion.Dialogue.Actions
{
    /// <summary>
    /// Changes enemy combat state when dialogue choice is selected.
    /// e.g. wrong answer -> AggressiveState, correct -> SparableState.
    /// </summary>
    public class SetStateDialogueAction : IDialogueAction
    {
        private readonly Enemy _enemy;
        private readonly ICombatState _newState;
        private readonly string _message;
        
        public SetStateDialogueAction(Enemy enemy, ICombatState newState, string message = "")
        {
            _enemy = enemy;
            _newState = newState;
            _message = message;
        }
        
        public string Execute(CombatContext? context)
        {
            _enemy.SetState(_newState);
            return _message;
        }
    }
}