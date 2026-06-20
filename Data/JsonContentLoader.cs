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
        }

        private void LoadRooms(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, RoomData>>(json);
            if (data == null) return;

            foreach (var (id, room) in data)
                _roomDescriptions[id] = room.Description;
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

                // store individual lines by their ID
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
                _itemDescriptions[id] = item.Description;
        }

        public string GetRoomDescription(string roomId)
        {
            return _roomDescriptions.TryGetValue(roomId, out var desc)
                ? desc
                : $"[Missing room description: {roomId}";
        }

        public List<string> GetDialogueLines(string entityId)
        {
            return _dialogueData.TryGetValue(entityId, out var lines)
                ? lines
                : new List<string>();
        }

        public string GetItemDescription(string itemId)
        {
            return _itemDescriptions.TryGetValue(itemId, out var desc)
                ? desc
                : $"[Missing item description: {itemId}]";
        }

        public string GetDialogueLine(string lineId)
        {
            return _dialogueLines.TryGetValue(lineId, out var line)
                ? line
                : $"[Missing dialogue line: {lineId}]";
        }
        
        // Private data models for JSON parsing

        private class RoomData
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        private class DialogueData
        {
            public List<string> Lines { get; set; }
            public Dictionary<string, string> LineMap { get; set; }
        }

        private class ItemData
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}