using HauntedMansion.Combat;
using HauntedMansion.Entities;

namespace HauntedMansion.Dialogue.Actions
{
    /// <summary>
    /// sets IsSparable flag
    /// </summary>
    public class SetStateDialogueAction : IDialogueAction
    {
        /// <summary>
        /// Command that sets the IsSparable flag on an enemy.
        /// </summary>
        private readonly Enemy _enemy;
        private readonly string _targetStateName;
        private readonly string _message;
        
        public SetStateDialogueAction(Enemy enemy, string targetStateName, string message = "")
        {
            _enemy = enemy;
            _targetStateName = targetStateName;
            _message = message;
        }
        
        public string Execute(CombatContext? context)
        {
            if (_targetStateName.Contains("Sparable", StringComparison.OrdinalIgnoreCase))
            {
                _enemy.IsSparable = true;
            }
            return _message;
        }
    }
}