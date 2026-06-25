using HauntedMansion.Combat.Interfaces;

namespace HauntedMansion.Combat.Actions
{
    /// <summary>
    /// No-op action. Used by SparableState when enemy stands down.
    /// </summary>
    public class IdleAction : IAction
    {
        private readonly string _message;
        
        // Constructor allows sets custom message on WHY the enemy is idle
        public IdleAction(string message = "The enemy does nothing.")
        {
            _message = message;
        }
        
        // zero damage result
        public CombatResult? Execute(CombatContext? context)
        {
            return new CombatResult
            {
                WasHit = false,
                DamageDealt = 0,
                Message = _message
            };
        }
    }
}