namespace HauntedMansion.Combat.Interfaces
{
    /// <summary>
    /// Represents a battle action (eg: attack, use item, flee)
    /// Used by CombatEngine and DialogueChoice
    /// </summary>
    public interface IAction
    {
        void Execute(CombatContext context);
    }
}