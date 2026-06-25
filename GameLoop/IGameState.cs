namespace HauntedMansion.GameLoop
{
    /// <summary>
    /// It forces every game screen (Exploration, Combat, Game Over) to implement a unified 
    /// lifecycle (Enter, Exit, Update).
    /// </summary>
    public interface IGameState
    {
        void OnEnter();
        void OnExit();
        
        // Called once per frame/turn by the GameManager
        void Update();
    }
}