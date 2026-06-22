namespace HauntedMansion.Interactions
{
    /// <summary>
    /// base interface for all objects the player can interact with
    /// Interact() returns a message string for IRenderer to display.
    /// No Console.WriteLine in implementations - all output via caller.
    /// </summary>
    public interface IInteractable
    {
        string Interact(Entities.Player player);
        string GetDescription();
    }
}