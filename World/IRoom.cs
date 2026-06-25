using HauntedMansion.Entities;
using HauntedMansion.Interactions;

namespace HauntedMansion.World
{
    /// <summary>
    /// Interface for all room types
    /// map and GameManager only interact with IRoom
    /// </summary>
    public interface IRoom
    {
        string GetRoomID();
        List<IInteractable> GetInteractables();
        List<Enemy> GetEnemies();
        string OnEnter(Player player);
        void ForceClearEnemies();

    }
}