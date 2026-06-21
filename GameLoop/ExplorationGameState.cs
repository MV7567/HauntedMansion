using HauntedMansion.Data;
using HauntedMansion.Entities;
using HauntedMansion.UI;
using HauntedMansion.UI.Commands;
using HauntedMansion.World;

namespace HauntedMansion.GameLoop
{
    /// <summary>
    /// player movement and room interaction
    /// calls try trigger encounter on each move
    /// switches to combat game state if encounter happens
    /// </summary>
    public class ExplorationGameState : IGameState
    {
        private readonly GameManager _manager;
        private readonly IContentLoader _loader;

        public ExplorationGameState(GameManager manager, IContentLoader loader)
        {
            _manager = manager;
            _loader = loader;
        }

        public void OnEnter()
        {
            _manager.Renderer.RenderRoom(
                _manager.Map.GetCurrentRoom(),
                _manager.Player);

            ShowExplorationMenu();
        }
        
        public void OnExit() { }
        
        public void HandleInput(ICommand command)
        {
            command.Execute();
        }

        /// <summary>
        /// show available actions in current room
        /// reads player input and creates command
        /// </summary>
        private void ShowExplorationMenu()
        {
            while (true)
            {
                var room = _manager.Map.GetCurrentRoom();
                var neighbours = _manager.Map.GetNeighbours(room.GetRoomID());
                var interactables = room.GetInteractables();

                var options = new List<string>();
                
                // movment options
                foreach (var neighbour in neighbours)
                    options.Add($"Go to {neighbour.GetRoomID().Replace('_', ' ')}");
                
                // interaction option
                foreach (var obj in interactables)
                    options.Add($"Examine: {obj.GetDescription()}");
                
                // always available
                options.Add("Open inventory");
                options.Add("Save game");
                options.Add("Quit");
                
                _manager.Renderer.RenderMenu("What do you do?", options);

                if (!int.TryParse(Console.ReadLine(), out int choice) ||
                    choice < 1 || choice > options.Count)
                {
                    _manager.Renderer.RenderMessage("Invalid choice");
                    continue;
                }

                choice--;
                
                // Handle movement
                if (choice < neighbours.Count)
                {
                    var target = neighbours[choice];
                    var moveCmd = new MoveCommand(
                        target.GetRoomID(), _manager.Map, _manager.Player);
                    moveCmd.Execute();
                    
                    // check for random encounter after moving
                    if (room is Room concreteRoom)
                    {
                        var encountered = concreteRoom
                            .TryTriggerEncounter(_manager.Player);

                        if (encountered != null)
                        {
                            _manager.ChangeState(new CombatGameState(
                                _manager,
                                new List<Enemy> { encountered },
                                loader));
                            return;
                        }
                    }
                    
                    // Refresh display after moving
                    _manager.Renderer.RenderRoom(
                        _manager.Map.GetCurrentRoom(), _manager.Player);
                    continue;
                }
                
                choice -= neighbours.Count;
                
                // Handle interactions
                if (choice < interactables.Count)
                {
                    interactables[choice].Interact(_manager.Player);
                    continue;
                }
                
                choice -= interactables.Count;
                
                // handle always available options
                switch (choice)
                {
                    case 0:
                        _manager.Renderer.RenderInventory(_manager.Player);
                        break;
                    case 1:
                        new SaveManager().SaveGame(_manager.Player, _manager.Map);
                        break;
                    case 2:
                        _manager.Quit();
                        return;
                }
            }
        }
    }
}