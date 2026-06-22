using HauntedMansion.Entities;

namespace HauntedMansion.World
{
    /// <summary>
    /// manages mansion layout as a graph of connected rooms
    /// </summary>
    public class Map
    {
        public enum PassageBlockReason { None, Locked, Blocked, RequiresItem }
        private Dictionary<string, PassageBlockReason> _lockedPassages = new();
        private Dictionary<string, string> _passageMessages = new();
        
        // all rooms keyed by RoomID
        private readonly Dictionary<string, IRoom> _rooms = new();
        
        // Adjacent rooms: maps RoomID to list of connected RoomIDs
        private readonly Dictionary<string, List<string>> _connections = new();
        
        // room locked until condition is met
       // private readonly HashSet<string> _lockedPassages = new();

        private IRoom _currentRoom;

        /// <summary>
        /// add room to map (called by room factory during map construction)
        /// </summary>
        public void AddRoom(IRoom room)
        {
            _rooms[room.GetRoomID()] = room;
            _connections[room.GetRoomID()] = new List<string>();
        }

        /// <summary>
        /// creates a 2-way connection between 2 rooms
        /// </summary>
        public void ConnectRooms(string roomIdA, string roomIdB)
        {
            if(_connections.ContainsKey(roomIdA))
                _connections[roomIdA].Add(roomIdB);
            
            if (_connections.ContainsKey(roomIdB))
                _connections[roomIdB].Add(roomIdA);
        }

        /// <summary>
        ///  locks/unlocks passages, by dialogue choices and/or key item interactions
        /// </summary>
        public void LockPassage(string fromId, string toId, 
            PassageBlockReason reason, string customMessage = null)
        {
            string key = $"{fromId}:{toId}";
            _lockedPassages[key] = reason;
            if (customMessage != null)
                _passageMessages[key] = customMessage;
        }
        
        public string GetBlockedMessage(string fromId, string toId)
        {
            string key = $"{fromId}:{toId}";
            if (_passageMessages.TryGetValue(key, out var msg)) return msg;
    
            var reason = _lockedPassages.TryGetValue(key, out var r) 
                ? r : PassageBlockReason.Blocked;
    
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
            _lockedPassages.Remove($"{fromId}:{toId}");
            _lockedPassages.Remove($"{toId}:{fromId}");
        }

        /// <summary>
        /// returns all rooms directly connected to the given room
        /// </summary>
        public List<IRoom> GetNeighbours(string roomId)
        {
            if (!_connections.ContainsKey(roomId))
                return new List<IRoom>();

            return _connections[roomId]
                .Where(id => _rooms.ContainsKey(id))
                .Select(id => _rooms[id])
                .ToList();
        }

        public IRoom GetCurrentRoom() => _currentRoom;

        /// <summary>
        /// checks if a connection exists and is unlocked
        /// </summary>
        /// <param name="fromId"></param>
        /// <param name="toId"></param>
        /// <returns></returns>
        public bool IsPassable(string fromId, string toId)
        {
            if (!_connections.ContainsKey(fromId)) return false;
            if (!_connections[fromId].Contains(toId)) return false;
            if (_lockedPassages.ContainsKey($"{fromId}:{toId}")) return false;
            return true;
        }

        /// <summary>
        /// allows passage, moves player to room, updates current room
        /// calls room.OnEnter(player), returns false if passage is blocked
        /// </summary>
        public (bool success, string message) MoveToRoom(string roomId, Player player)
        {
            string currentId = _currentRoom?.GetRoomID();

            if (currentId != null && !IsPassable(currentId, roomId))
                return (false, GetBlockedMessage(currentId, roomId));

            if (!_rooms.ContainsKey(roomId))
                return (false, $"[ERROR] Room '{roomId}' does not exist.");

            _currentRoom = _rooms[roomId];
            string description = _currentRoom.OnEnter(player);
            return (true, description);
        }

        /// <summary>
        /// sets the starting room without triggering OnEnter()
        /// called at game start before the player was placed
        /// </summary>
        public void SetStartingRoom(string roomId)
        {
            if (_rooms.ContainsKey(roomId))
                _currentRoom = _rooms[roomId];
        }

        public bool IsFullyCleared()
        {
            return _rooms.Values.OfType<Room>().All(r => r.IsCleared);
        }
    }
}