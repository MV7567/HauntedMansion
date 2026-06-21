using System.Text.Json;

namespace HauntedMansion.Data
{
    /// <summary>
    /// Loads all JSON content files at startup
    /// </summary>
    public class JsonContentLoader : IContentLoader
    {
        private Dictionary<string, string> _roomDescriptions = new();
        private Dictionary<string, List<string>> _dialogueData = new();
        private Dictionary<string, string> _itemDescriptions = new();
        private Dictionary<string, string> _dialogueLines = new();
        private Dictionary<string, EnemyData> _enemyData = new();
        private Dictionary<string, RoomData> _roomData = new();
        
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
        }

        private void LoadRooms(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, RoomData>>(json);
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
            var data = JsonSerializer.Deserialize<Dictionary<string, DialogueData>>(json);
            if (data == null) return;

            foreach (var (id, dialogue) in data)
            {
                _dialogueData[id] = dialogue.Lines ?? new List<string>();

                if (dialogue.LineMap != null)
                    foreach (var (lineId, text) in dialogue.LineMap)
                        _dialogueLines[lineId] = text;
            }
        }

        private void LoadItems(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, ItemData>>(json);
            if (data == null) return;

            foreach (var (id, item) in data)
                _itemDescriptions[id] = item.Description ?? "";
        }
        
        private void LoadEnemies(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var json = File.ReadAllText(filePath);
            _enemyData = JsonSerializer.Deserialize
                <Dictionary<string, EnemyData>>(json) ?? new();
        }

        public string GetRoomDescription(string roomId) =>
            _roomDescriptions.TryGetValue(roomId, out var desc)
                ? desc
                : $"[Missing room description: {roomId}]";

        public List<string> GetDialogueLines(string entityId) =>
                    _dialogueData.TryGetValue(entityId, out var lines)
                        ? lines
                        : new List<string>();

        public string GetItemDescription(string itemId) =>
            _itemDescriptions.TryGetValue(itemId, out var desc)
                ? desc
                : $"[Missing item description: {itemId}]";

        public string GetDialogueLine(string lineId) =>
            _dialogueLines.TryGetValue(lineId, out var line)
                ? line
                : $"[Missing dialogue line: {lineId}]";
        
        public EnemyData? GetEnemyData(string enemyId) =>
            _enemyData.TryGetValue(enemyId, out var d) ? d : null;

        public RoomData? GetRoomData(string roomId) =>
            _roomData.TryGetValue(roomId, out var d) ? d : null;

        public List<string> GetAllRoomIds() => _roomData.Keys.ToList();
     
        
        
        // Private data models for JSON parsing
        
        private class DialogueData
        {
            public List<string>? Lines { get; set; }
            public Dictionary<string, string>? LineMap { get; set; }
        }
        
        private class ItemData
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
        }
        
        public class EnemyData
        {
            public string? Name { get; set; }
            public string? Type { get; set; }
            public float EncounterWeight { get; set; }
            public string? StartingNodeId { get; set; }
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
    }
}