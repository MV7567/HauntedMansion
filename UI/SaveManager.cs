using System.Text.Json;
using HauntedMansion.Core;
using HauntedMansion.Entities;
using HauntedMansion.World;

namespace HauntedMansion.UI
{
    /// <summary>
    /// saves layer stats and map room cleared states to file
    /// </summary>
    public class SaveManager
    {
        private const string SavePath = "save.json";

        public string SaveGame(Player player, Map map)
        {
            var saveData = new SaveData
            {
                PlayerName = player.Name,
                CurrentHP = player.CurrentHP,
                Experience = player.Experience,
                Money = player.Money,
                CurrentRoomId = map.GetCurrentRoom()?.GetRoomID()
            };
            
            var json = JsonSerializer.Serialize(saveData,
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(SavePath, json);
            return "Game saved.";
        }

        public (SaveData? data, string message) LoadGame()
        {
            if (!File.Exists(SavePath))
                return (null, "No save file found.");

            var json = File.ReadAllText(SavePath);
            var data = JsonSerializer.Deserialize<SaveData>(json);
            return data == null
                ? (null, "Failed to load save file.")
                : (data, "Game loaded.");
        }
        
        public bool HasSaveFile() => File.Exists(SavePath);

        public class SaveData
        {
            public string? PlayerName { get; set; }
            public int CurrentHP { get; set; }
            public int Experience { get; set; }
            public int Money { get; set; }
            public string? CurrentRoomId { get; set; }
        }
    }
}