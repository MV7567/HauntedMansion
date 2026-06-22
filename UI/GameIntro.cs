using HauntedMansion.Data;
using HauntedMansion.Entities;
using HauntedMansion.Core;

namespace HauntedMansion.UI
{
    /// <summary>
    /// Handles all pre-game UI: title screen, name entry, intro text.
    /// Keeps Program.cs clean and all player interaction in UI layer.
    /// </summary>
    public class GameIntro
    {
        private readonly IRenderer _renderer;
        private readonly SaveManager _saveManager = new();
        
        public GameIntro(IRenderer renderer)
        {
            _renderer = renderer;
        }

        /// <summary>
        /// Shows main menu. Returns (player, startRoomId) to Program.cs.
        /// Handles both new game and load game paths.
        /// </summary>
        public (Player player, string startRoomId) ShowMainMenu(IContentLoader loader)
        {
            _renderer.RenderMessage("================================");
            _renderer.RenderMessage("       HAUNTED MANSION          ");
            _renderer.RenderMessage("================================");
            //_renderer.RenderMessage("\nEnter your name: ");

            var options = new List<string> { "New Game" };
            if (_saveManager.HasSaveFile())
                options.Add("Load Game");
            options.Add("Quit");

            _renderer.RenderMenu("", options);

            if (!int.TryParse(Console.ReadLine(), out int choice))
                choice = 1;
            
            // Quit
            if (choice == options.Count)
                Environment.Exit(0);
            
            // Load Game
            if (choice == 2 && _saveManager.HasSaveFile())
            {
                var (saveData, msg) = _saveManager.LoadGame();
                _renderer.RenderMessage(msg);

                if (saveData != null)
                {
                    var statsData = loader.GetPlayerDefaultStats();
                    var baseStats = new CharacterStats(
                        statsData.Attack, statsData.Defence, statsData.Magic,
                        statsData.Speed, statsData.Accuracy, statsData.MaxHP);
                    
                    var player = new Player(
                        saveData.PlayerName ?? "Stranger", baseStats);
                    
                    // Restore player state
                    if (saveData.Money > 0)
                        player.AddMoney(saveData.Money);
                    if (saveData.Experience > 0)
                        player.GainExperience(saveData.Experience);

                    return (player, saveData.CurrentRoomId ?? "entrance_hall");
                }
            }
            
            // New Game
            return (CreateNewPlayer(loader), "entrance_hall");
        }

        private Player CreateNewPlayer(IContentLoader loader)
        {
            _renderer.RenderMessage("\nEnter your name: ");
            var name = Console.ReadLine()?.Trim() is { Length: > 0 } n
                ? n : "Stranger";
            
            var statsData = loader.GetPlayerDefaultStats();
            var baseStats = new CharacterStats(
                statsData.Attack, statsData.Defence, statsData.Magic,
                statsData.Speed, statsData.Accuracy, statsData.MaxHP);
            
            var player = new Player(name, baseStats);
            
            _renderer.RenderMessage($"\nYou open your eyes, {name}.");
            _renderer.RenderMessage(
                "The air smells of dust and decay.");
            _renderer.RenderMessage(
                "You are in a mansion. You don't know how you got here.");
            _renderer.RenderMessage("\nPress Enter to continue...");
            Console.ReadLine();

            return player;
        }
        
        // Keep for backwards compatibility
        public string GetPlayerName()
        {
            _renderer.RenderMessage("\nEnter your name: ");
            return Console.ReadLine()?.Trim() is { Length: > 0 } n
                ? n : "Stranger";
        }
        
        public void RenderIntro(string playerName)
        {
            _renderer.RenderMessage($"\nYou open your eyes, {playerName}.");
            _renderer.RenderMessage("The air smells of dust and decay.");
            _renderer.RenderMessage(
                "You are in a mansion. You don't know how you got here.");
            _renderer.RenderMessage("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}