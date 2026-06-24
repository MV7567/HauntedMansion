namespace HauntedMansion.GameLoop
{
    public class GameWonState : IGameState
    {
        private readonly GameManager _manager;
        
        public GameWonState(GameManager manager)
        {
            _manager = manager;
        }
        
        public void OnEnter() { }
        public void OnExit() { }
        
        public void Update()
        {
            _manager.Renderer.ClearScreen();
            _manager.Renderer.RenderMessage("===============================================");
            _manager.Renderer.RenderMessage("                  VICTORY!                     ");
            _manager.Renderer.RenderMessage("===============================================");
            _manager.Renderer.RenderMessage("\nYou unlock the heavy basement door with the Old Key.");
            _manager.Renderer.RenderMessage("Behind it, you find an old smuggling tunnel leading outside.");
            _manager.Renderer.RenderMessage("You have successfully escaped the Haunted Mansion!");
            _manager.Renderer.RenderMessage("\nThank you for playing.");
            
            _manager.WaitToContinue();
            _manager.Quit();
        }

    }

}