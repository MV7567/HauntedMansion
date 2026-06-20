namespace HauntedMansion.Interactions
{
    /// <summary>
    /// base interface for all objects the player can interact with
    /// </summary>
    public interface IInteractable
    {
        void Interact(Entities.Player player);
        string GetDescription();
    }
}