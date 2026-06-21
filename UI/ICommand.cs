using HauntedMansion.Combat;

namespace HauntedMansion.UI
{
    /// <summary>
    /// player input, dialogue effects and UI actions are ICommand objects
    /// </summary>
    public interface ICommand
    {
        void Execute(CombatContext context = null);
    }
}