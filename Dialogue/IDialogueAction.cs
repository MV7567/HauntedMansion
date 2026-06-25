using HauntedMansion.Combat;

namespace HauntedMansion.Dialogue
{
    /// <summary>
    /// Action executed when a dialogue choice is selected.
    /// Separate from ICommand (UI).
    /// Returns a message string for IRenderer to display.
    /// </summary>
    public interface IDialogueAction
    {
        string Execute(CombatContext? context);
    }
}