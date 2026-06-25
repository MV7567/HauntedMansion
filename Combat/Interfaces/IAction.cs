namespace HauntedMansion.Combat.Interfaces
{
    /// <summary>
    /// Represents a battle action (eg: attack, use item, flee)
    /// Used by CombatEngine and DialogueChoice
    /// encapsulates an action as an object, CombatGameState 
    /// executes it
    /// </summary>
    public interface IAction
    {
        //Returns a CombatResult with damage dealt and UI messages
        CombatResult? Execute(CombatContext? context);
    }
}