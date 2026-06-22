using System;
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
        private IGameState _nextState;
        private bool _isRunning;

        public GameManager(Player player, Map map, IRenderer renderer, IInputProvider input)
        {
            Player = player;
            Map = map;
            Renderer = renderer;
            Input = input;
        }

        public void ChangeState(IGameState newState)
        {
            _nextState = newState;
        }

        public void Run(IGameState startingState)
        {
            _isRunning = true;
            _nextState = startingState;

            while (_isRunning)
            {
                if (_nextState != null)
                {
                    _currentGameState?.OnExit();
                    _currentGameState = _nextState;
                    _nextState = null;
                    _currentGameState.OnEnter();
                }

                _currentGameState?.Update();
            }
        }
        
        public void Quit() 
        {
            _isRunning = false;
            Environment.Exit(0);
        }
    }
}