using HauntedMansion.Combat;
using HauntedMansion.Data;
using HauntedMansion.Entities;
using HauntedMansion.UI;
using HauntedMansion.World;

namespace HauntedMansion.GameLoop
{
    /// <summary>
    /// controls the game loop
    /// delegates HandleInput() to CurrentGameState
    /// one instance
    /// </summary>
    public class GameManager
    {
        // Core systems
        public Player Player { get; private set; }
        public Map Map { get; private set; }
        public IRenderer Renderer { get; private set; }

        private IGameState _currentGameState;
        private bool _isRunning;

        public GameManager(Player player, Map map, IRenderer renderer)
        {
            Player = player;
            Map = map;
            Renderer = renderer;
        }

        /// <summary>
        /// Calls OnExit() on old state, sets new state, calls OnEnter()
        /// </summary>
        public void ChangeState(IGameState newState)
        {
            _currentGameState?.OnExit();
            _currentGameState = newState;
            _currentGameState.OnEnter();
        }

        public void ProcessInput(ICommand command)
        {
            _currentGameState?.HandleInput(command);
        }

        public void Run(IGameState startingState)
        {
            _isRunning = true;
            ChangeState(startingState);

            while (_isRunning)
            {
                Update();
            }
        }
        
        public void Quit() => _isRunning = false;

        private void Update()
        {
            // future: time based updates if needed
            // currently input driven only
        }
    }
}