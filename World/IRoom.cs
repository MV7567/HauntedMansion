using HauntedMansion.Entities;
using HauntedMansion.Interactions;

namespace HauntedMansion.World
{
    /// <summary>
    /// Interface for all room types
    /// </summary>
    public interface IRoom
    {
        string GetRoomID();
        List<IInteractable> GetInteractables();
        List<Enemy> GetEnemies();
        void OnEnter(Player player);

    }
}