using System.Text.Json;
using HauntedMansion.Entities;
using HauntedMansion.World;
using HauntedMansion.Inventory.Interfaces;
using HauntedMansion.Data;

namespace HauntedMansion.UI
{
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
            if (!File.Exists(SavePath)) return (null, "No save file found.");

            var json = File.ReadAllText(SavePath);
            var data = JsonSerializer.Deserialize<SaveData>(json);
            return data == null ? (null, "Failed to load save file.") : (data, "Game loaded.");
        }
        
        public bool HasSaveFile() => File.Exists(SavePath);
        
        public string ApplySaveData(Player player, Map map, IContentLoader loader, SaveData data)
        {
            player.TakeDamage(player.CurrentHP - data.CurrentHP);
            player.AddMoney(data.Money - player.Money);
            player.GainExperience(data.Experience - player.Experience);
            
            if (data.ClearedRooms != null)
                foreach (var id in data.ClearedRooms) (map.GetRoom(id) as Room)?.ForceClearEnemies();
                
            if (data.LootedRooms != null)
                foreach (var id in data.LootedRooms) (map.GetRoom(id) as Room)?.ForceLootAll();

            var itemFactory = new ItemFactory(loader);
            
            if (data.InventoryIds != null)
                foreach (var itemId in data.InventoryIds)
                {
                    var item = itemFactory.CreateItem(itemId);
                    if (item != null) player.PlayerInventory.AddItem(item);
                }

            if (data.EquippedIds != null)
                foreach (var kvp in data.EquippedIds)
                {
                    var item = itemFactory.CreateItem(kvp.Value) as IEquippable;
                    if (item != null) player.Equipment.Equip(item, player);
                }

            return data.CurrentRoomId ?? "entrance_hall";
        }

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