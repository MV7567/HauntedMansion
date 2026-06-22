using System.Collections.Generic;
using System.Linq;
using HauntedMansion.Entities;

namespace HauntedMansion.World
{
    public record struct Edge(string From, string To);

    public class Map
    {
        public enum PassageBlockReason { None, Locked, Blocked, RequiresItem }
        
        private readonly Dictionary<Edge, PassageBlockReason> _lockedPassages = new();
        private readonly Dictionary<Edge, string> _passageMessages = new();
        private readonly Dictionary<string, IRoom> _rooms = new();
        private readonly Dictionary<string, List<string>> _connections = new();
        private IRoom _currentRoom;

        public void AddRoom(IRoom room)
        {
            _rooms[room.GetRoomID()] = room;
            _connections[room.GetRoomID()] = new List<string>();
        }

        public void ConnectRooms(string roomIdA, string roomIdB)
        {
            if(_connections.ContainsKey(roomIdA)) _connections[roomIdA].Add(roomIdB);
            if (_connections.ContainsKey(roomIdB)) _connections[roomIdB].Add(roomIdA);
        }

        public void LockPassage(string fromId, string toId, PassageBlockReason reason, string customMessage = null)
        {
            var edge = new Edge(fromId, toId);
            _lockedPassages[edge] = reason;
            if (customMessage != null) _passageMessages[edge] = customMessage;
        }
        
        public string GetBlockedMessage(string fromId, string toId)
        {
            var edge = new Edge(fromId, toId);
            if (_passageMessages.TryGetValue(edge, out var msg)) return msg;
    
            var reason = _lockedPassages.TryGetValue(edge, out var r) ? r : PassageBlockReason.Blocked;
            return reason switch
            {
                PassageBlockReason.Locked  => "The door is locked.",
                PassageBlockReason.Blocked => "Something is blocking the door from the other side.",
                PassageBlockReason.RequiresItem => "You need something to open this.",
                _ => "The way is blocked."
            };
        }

        public void UnlockPassage(string fromId, string toId)
        {
            _lockedPassages.Remove(new Edge(fromId, toId));
            _lockedPassages.Remove(new Edge(toId, fromId));
        }

        public List<IRoom> GetNeighbours(string roomId)
        {
            if (!_connections.ContainsKey(roomId)) return new List<IRoom>();
            return _connections[roomId].Where(id => _rooms.ContainsKey(id)).Select(id => _rooms[id]).ToList();
        }

        public IRoom GetCurrentRoom() => _currentRoom;

        public bool IsPassable(string fromId, string toId)
        {
            if (!_connections.ContainsKey(fromId) || !_connections[fromId].Contains(toId)) return false;
            if (_lockedPassages.ContainsKey(new Edge(fromId, toId))) return false;
            return true;
        }

        public (bool success, string message) MoveToRoom(string roomId, Player player)
        {
            string currentId = _currentRoom?.GetRoomID();
            if (currentId != null && !IsPassable(currentId, roomId))
                return (false, GetBlockedMessage(currentId, roomId));

            if (!_rooms.ContainsKey(roomId))
                return (false, $"[ERROR] Room '{roomId}' does not exist.");

            _currentRoom = _rooms[roomId];
            return (true, string.Empty);
        }

        public void SetStartingRoom(string roomId)
        {
            if (_rooms.ContainsKey(roomId)) _currentRoom = _rooms[roomId];
        }

        public bool IsFullyCleared() => _rooms.Values.OfType<Room>().All(r => r.IsCleared);
        public IEnumerable<IRoom> GetAllRooms() => _rooms.Values;
        public IRoom? GetRoom(string roomId) => _rooms.TryGetValue(roomId, out var r) ? r : null;
    }
}