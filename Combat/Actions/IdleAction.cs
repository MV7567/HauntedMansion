using HauntedMansion.Combat.Interfaces;

namespace HauntedMansion.Combat.Actions
{
    /// <summary>
    /// No-op action. Used by SparableState when enemy stands down.
    /// </summary>
    public class IdleAction : IAction
    {
        private readonly string _message;
        
        public IdleAction(string message = "The enemy does nothing.")
        {
            _message = message;
        }
        
        public void Execute(CombatContext context)
        {
            Console.WriteLine(_message);
        }
    }
}