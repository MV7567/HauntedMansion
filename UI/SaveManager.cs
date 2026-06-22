using System.Text.Json;
using System.Linq;
using HauntedMansion.Entities;
using HauntedMansion.World;
using System.Collections.Generic;
using System.IO;
using HauntedMansion.Inventory.Interfaces;

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
            var inventoryIds = new List<string>();
            inventoryIds.AddRange(player.PlayerInventory.GetConsumables().Select(i => i.ID));
            inventoryIds.AddRange(player.PlayerInventory.GetKeyItem().Select(i => i.ID));
            inventoryIds.AddRange(player.PlayerInventory.GetEquippables().Select(i => i.ID));

            var equippedIds = new Dictionary<EquipmentSlot, string>();
            foreach(EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                var item = player.Equipment.GetSlot(slot);
                if (item != null) equippedIds[slot] = item.ID;
            }

            var saveData = new SaveData
            {
                PlayerName = player.Name,
                CurrentHP = player.CurrentHP,
                Experience = player.Experience,
                Money = player.Money,
                CurrentRoomId = map.GetCurrentRoom()?.GetRoomID(),
                ClearedRooms = map.GetAllRooms().Where(r => r.GetEnemies().Count == 0).Select(r => r.GetRoomID()).ToList(),
                LootedRooms = map.GetAllRooms().Where(r => r is Room cr && cr.IsFullyLooted()).Select(r => r.GetRoomID()).ToList(),
                InventoryIds = inventoryIds,
                EquippedIds = equippedIds
            };
            
            var json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
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
            public List<string> ClearedRooms { get; set; } = new();
            public List<string> LootedRooms { get; set; } = new();
            public List<string> InventoryIds { get; set; } = new();
            public Dictionary<EquipmentSlot, string> EquippedIds { get; set; } = new();
        }
    }
}