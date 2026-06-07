using System.Text.Json;

namespace HauntedMansion.Data
{
    public class JsonContentLoader : IContentLoader
    {
        private Dictionary<string, string> _roomDescriptions = new();
        private Dictionary<string, List<string>> _dialogueData = new();
        private Dictionary<string, string> _itemDescriptions = new();
        private Dictionary<string, string> _dialogueLines = new();

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
        private void LoadItems(string filePath) {}
    }
}