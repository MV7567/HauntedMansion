using HauntedMansion.Data;
using HauntedMansion.Entities;
using HauntedMansion.Core;
using System;
using System.Collections.Generic;

namespace HauntedMansion.UI
{
    public class GameIntro
    {
        private readonly IRenderer _renderer;
        private readonly IInputProvider _input;
        private readonly SaveManager _saveManager = new();
        
        public GameIntro(IRenderer renderer, IInputProvider input)
        {
            _renderer = renderer;
            _input = input;
        }

        public (Player player, string startRoomId) ShowMainMenu(IContentLoader loader)
        {
            // CLEAR SCREEN: Clean console before showing the main title
            _renderer.ClearScreen();
            
            _renderer.RenderMessage("================================");
            _renderer.RenderMessage("       HAUNTED MANSION          ");
            _renderer.RenderMessage("================================");

            var options = new List<string> { "New Game" };
            if (_saveManager.HasSaveFile())
                options.Add("Load Game");
            options.Add("Quit");

            _renderer.RenderMenu("", options);

            int choice = _input.GetIntInput(1, options.Count);
            
            if (choice == options.Count) Environment.Exit(0);
            
            if (options.Count == 3 && choice == 2 && _saveManager.HasSaveFile())
            {
                var (saveData, msg) = _saveManager.LoadGame();
                _renderer.RenderMessage(msg);

                if (saveData != null)
                {
                    var statsData = loader.GetPlayerDefaultStats();
                    var baseStats = new CharacterStats(
                        statsData.Attack, statsData.Defence, statsData.Magic,
                        statsData.Speed, statsData.Accuracy, statsData.MaxHP);
                    
                    var player = new Player(saveData.PlayerName ?? "Stranger", baseStats);
                    
                    if (saveData.Money > 0) player.AddMoney(saveData.Money);
                    if (saveData.Experience > 0) player.GainExperience(saveData.Experience);

                    return (player, saveData.CurrentRoomId ?? "entrance_hall");
                }
            }
            
            return (CreateNewPlayer(loader), "entrance_hall");
        }

        private Player CreateNewPlayer(IContentLoader loader)
        {
            _renderer.RenderMessage("\nEnter your name: ");
            var name = Console.ReadLine()?.Trim() is { Length: > 0 } n ? n : "Stranger";
            
            var statsData = loader.GetPlayerDefaultStats();
            var baseStats = new CharacterStats(
                statsData.Attack, statsData.Defence, statsData.Magic,
                statsData.Speed, statsData.Accuracy, statsData.MaxHP);
            
            var player = new Player(name, baseStats);
            

            _renderer.ClearScreen();
            
            _renderer.RenderMessage($"\nYou open your eyes, {name}.");
            _renderer.RenderMessage("The air smells of dust and decay.");
            _renderer.RenderMessage("You are in a mansion. You don't know how you got here.");
            
            _renderer.RenderContinuePrompt();
            _input.WaitForContinue();

            return player;
        }
    }
}