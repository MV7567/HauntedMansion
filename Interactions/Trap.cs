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
            _isTriggered = false;
        }

        public void Interact(Player player)
        {
            if (_isTriggered)
            {
                Console.WriteLine("The trap has already been triggered.");
                return;
            }
            
            player.TakeDamage(_damageAmount);
            _isTriggered = true; 
            Console.WriteLine($"A trap! You took {_damageAmount} damage!");
            
            if (!player.IsAlive())
                Console.WriteLine("You died...");
        }

        public string GetDescription()
        {
            return _isTriggered ? "A disarmed trap." : _description;
        }
    }
}