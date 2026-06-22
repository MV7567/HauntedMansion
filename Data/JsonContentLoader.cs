using System.Text.Json;

namespace HauntedMansion.Data
{
    /// <summary>
    /// Loads all JSON content files at startup
    /// </summary>
    public class JsonContentLoader : IContentLoader
    {
        private Dictionary<string, string> _roomDescriptions = new();
        private Dictionary<string, ItemData> _itemData = new();
        private Dictionary<string, EnemyData> _enemyData = new();
        private Dictionary<string, RoomData> _roomData = new();
        private Dictionary<string, List<InteractableData>> _interactableData = new();
        private Dictionary<string, DialogueFileData> _dialogueData = new();
        
        private JsonContentLoader.StatsData _playerDefaultStats;
        
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        };
        
        /// <summary>
        /// Load all files from the given directory
        /// called once at game startup by GameManager
        /// before room or enemy creation
        /// </summary>
        public void LoadAll(string path)
        {
            LoadRooms(Path.Combine(path, "rooms.json"));
            LoadDialogue(Path.Combine(path, "dialogue.json"));
            LoadItems(Path.Combine(path, "items.json"));
            LoadEnemies(Path.Combine(path, "enemies.json"));
            LoadInteractables(Path.Combine(path, "interactables.json"));
            
            LoadPlayer(Path.Combine(path, "player.json"));
        }

        private void LoadRooms(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Lacking data file: {filePath}");
                
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, RoomData>>(json, _jsonOptions);
            if (data == null) return;

            foreach (var (id, room) in data)
            {
                _roomData[id] = room;
                _roomDescriptions[id] = room.Description ?? "";
            }
        }

        private void LoadDialogue(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, DialogueFileData>>(json, _jsonOptions);
            if (data == null) return;

            foreach (var (id, dialogue) in data)
                _dialogueData[id] = dialogue;
        }

        private void LoadItems(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, ItemData>>(json, _jsonOptions);
            if (data == null) return;

            foreach (var (id, item) in data)
                _itemData[id] = item;
        }
        
        private void LoadEnemies(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var json = File.ReadAllText(filePath);
            _enemyData = JsonSerializer.Deserialize
                <Dictionary<string, EnemyData>>(json, _jsonOptions) ?? new();
        }
        
        private void LoadPlayer(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<PlayerFileData>(json, _jsonOptions);
            _playerDefaultStats = data?.DefaultStats;
        }
        
        private void LoadInteractables(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var json = File.ReadAllText(filePath);
            _interactableData = JsonSerializer.Deserialize
                <Dictionary<string, List<InteractableData>>>(json, _jsonOptions) ?? new();
        }

        public string GetRoomDescription(string roomId) =>
            _roomDescriptions.TryGetValue(roomId, out var desc)
                ? desc
                : $"[Missing room description: {roomId}]";
        
        
        public ItemData? GetItemData(string itemId) =>
            _itemData.TryGetValue(itemId, out var d) ? d : null;
        
        
        public DialogueFileData? GetDialogueData(string entityId) => 
            _dialogueData.TryGetValue(entityId, out var d) ? d : null;
        
        public EnemyData? GetEnemyData(string enemyId) =>
            _enemyData.TryGetValue(enemyId, out var d) ? d : null;

        public RoomData? GetRoomData(string roomId) =>
            _roomData.TryGetValue(roomId, out var d) ? d : null;
        
        public List<InteractableData> GetInteractables(string roomId) =>
            _interactableData.TryGetValue(roomId, out var data) ? data : new();

        public List<string> GetAllRoomIds() => _roomData.Keys.ToList();
        
        public StatsData GetPlayerDefaultStats() => 
            _playerDefaultStats ?? new StatsData 
                { Attack=10, Defence=10, Magic=10, Speed=10, Accuracy=75, MaxHP=100 };
     
        
        
        // Private data models for JSON parsing
        
        public class DialogueFileData
        {
            public List<string>? Lines { get; set; }
            public Dictionary<string, DialogueNodeData>? Nodes { get; set; }
        }

        public class DialogueNodeData
        {
            public string? Text { get; set; }
            public List<DialogueChoiceData>? Choices { get; set; }
        }

        public class DialogueChoiceData
        {
            public string? Text { get; set; }
            public string? NextNodeId { get; set; }
            public string? RequiredItem { get; set; }
            public string? ActionType { get; set; } 
            public string? State { get; set; } 
            public string? ActionMessage { get; set; }
        }
        
        public class ItemData
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Slot { get; set; }
            public int HealAmount { get; set; }
            public StatsData? Stats { get; set; }
        }
        
        public class EnemyData
        {
            public string? Name { get; set; }
            public string? Type { get; set; }
            public float EncounterWeight { get; set; }
            public string? StartingNodeId { get; set; }
            public string? postBattleNodeId { get; set; }
            public string? BaseTreeId { get; set; }
            public StatsData? Stats { get; set; }
            public List<BodyPartData>? BodyParts { get; set; }
        }
        
        public class StatsData
        {
            public int Attack { get; set; }
            public int Defence { get; set; }
            public int Magic { get; set; }
            public int Speed { get; set; }
            public int Accuracy { get; set; }
            public int MaxHP { get; set; }
        }
        
        public class BodyPartData
        {
            public string? Type { get; set; }
            public float HitMod { get; set; }
            public float DmgMult { get; set; }
        }
        
        public class RoomData
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public List<string> Enemies { get; set; } = new();
            public string? Boss { get; set; }
            public List<string> Connections { get; set; } = new();
        }
        
        private class PlayerFileData
        {
            public StatsData? DefaultStats { get; set; }
        }
        
        public class InteractableData
        {
            public string? Type { get; set; }
            public string? Description { get; set; }
            public List<string> Items { get; set; } = new();
            public int Money { get; set; }
            public int Damage { get; set; }
            public string? NpcName { get; set; }
            public List<ShopStockData> Stock { get; set; } = new();
        }
        
        public class ShopStockData
        {
            public string? Item { get; set; }
            public int Price { get; set; }
        }
    }
}