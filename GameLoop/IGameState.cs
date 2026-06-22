namespace HauntedMansion.GameLoop
{
    public interface IGameState
    {
        void OnEnter();
        void OnExit();
    }
}