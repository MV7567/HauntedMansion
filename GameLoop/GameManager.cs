using HauntedMansion.Entities;
using HauntedMansion.UI;
using HauntedMansion.World;

namespace HauntedMansion.GameLoop
{
    public class GameManager
    {
        public Player Player { get; }
        public Map Map { get; }
        public IRenderer Renderer { get; }
        public IInputProvider Input { get; }

        private IGameState _currentGameState;

        public GameManager(Player player, Map map, IRenderer renderer, IInputProvider input)
        {
            Player = player;
            Map = map;
            Renderer = renderer;
            Input = input;
        }

        public void ChangeState(IGameState newState)
        {
            _currentGameState?.OnExit();
            _currentGameState = newState;
            _currentGameState.OnEnter();
        }

        public void Run(IGameState startingState)
        {
            ChangeState(startingState);
        }
        
        public void Quit() => Environment.Exit(0);
    }
}