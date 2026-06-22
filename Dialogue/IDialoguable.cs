namespace HauntedMansion.Dialogue
{
    /// <summary>
    /// An entity has a dialogue tree
    /// gives dialogue engine with the first node ID to load
    /// </summary>
    public interface IDialoguable
    {
        string GetStartingNode();
        string GetBaseTreeId();
    }
}