using HauntedMansion.Entities;

namespace HauntedMansion.Interactions
{
    /// <summary>
    /// environmental hazard that damages player on interaction
    /// Bypassed combat engine to do damage directly (TakeDamage())
    /// can be triggered automatically by room movment
    /// </summary>
    public class Trap : IInteractable
    {
        private readonly string _description;
        private readonly int _damageAmount;
        private bool _isTriggered;

        public Trap(string description, int damageAmount)
        {
            _description = description;
            _damageAmount = damageAmount;
        }

        public string Interact(Player player)
        {
            if (_isTriggered)
                return "The trap has already been triggered.";
            
            // Direct damage to the player
            player.TakeDamage(_damageAmount);
            _isTriggered = true;

            return !player.IsAlive()
                ? $"A trap! You took {_damageAmount} damage! You died..."
                : $"A trap! You took {_damageAmount} damage!";
        }
        public string GetDescription() =>
            _isTriggered ? "A disarmed trap." : _description;
    }
}