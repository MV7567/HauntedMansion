using HauntedMansion.Entities;
using HauntedMansion.World;

namespace HauntedMansion.Data
{
    /// <summary>
    /// Builds Room instances from JSON data.
    /// Delegates enemy creation to EnemyFactory,
    /// interactable creation to InteractableFactory.
    /// Adding a new room requires only a JSON entry (OCP).
    /// </summary>
    public class RoomFactory
    {
        private readonly IContentLoader _loader;
        private readonly EnemyFactory _enemyFactory;
        private readonly InteractableFactory _interactableFactory;

        public RoomFactory(IContentLoader loader)
        {
            _loader = loader;
            _enemyFactory = new EnemyFactory(loader);
            var itemFactory = new ItemFactory(loader);
            _interactableFactory = new InteractableFactory(loader, itemFactory);
        }

        /// <summary>
        /// create a fully configured room by ID
        /// Adding a new room, linking it to the map, populating it with enemies
        /// </summary>
        public Room? CreateRoom(string roomId)
        {
            var data = _loader.GetRoomData(roomId);
            if (data == null) return null;
            
            var normalEnemies = data.Enemies
                .Select(id => _enemyFactory.CreateEnemy(id) as NormalEnemy)
                .Where(e => e != null)
                .Cast<NormalEnemy>()
                .ToList();
            
            BossEnemy? boss = null;
            if (!string.IsNullOrEmpty(data.Boss))
                boss = _enemyFactory.CreateEnemy(data.Boss) as BossEnemy;
            
            var room = new Room(roomId, normalEnemies, boss, _loader);
            
            foreach (var interactable in _interactableFactory.CreateForRoom(roomId))
                room.AddInteractable(interactable);
            
            return room;
        }

        /// <summary>
        /// Builds entire map from rooms.json.
        /// Creates all rooms and connects them automatically.
        /// </summary>
        public Map BuildMap(string startingRoomId)
        {
            var map = new Map();
            
            // 1. create all rooms
            foreach (var roomId in _loader.GetAllRoomIds())
            {
                var room = CreateRoom(roomId);
                if (room != null)
                    map.AddRoom(room);
            }
            
            // 2. connect rooms
            var connected = new HashSet<string>();
            foreach (var roomId in _loader.GetAllRoomIds())
            {
                var data = _loader.GetRoomData(roomId);
                if (data?.Connections == null) continue;

                foreach (var neighbourId in data.Connections)
                {
                    // key - both directions
                    string key = string.Compare(roomId, neighbourId) < 0
                        ? $"{roomId}:{neighbourId}"
                        : $"{neighbourId}:{roomId}";

                    if (connected.Add(key))
                        map.ConnectRooms(roomId, neighbourId);
                }
            }
            map.SetStartingRoom(startingRoomId);
            return map;
        }
    }
}