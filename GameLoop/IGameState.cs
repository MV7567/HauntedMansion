using HauntedMansion.UI;

namespace HauntedMansion.GameLoop
{
    public interface IGameState
    {
        void HandleInput(ICommand command);
        void OnEnter();
        void OnExit();
    }
}