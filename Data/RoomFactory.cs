using HauntedMansion.World;
using HauntedMansion.Entities;

namespace HauntedMansion.Data
{
    /// <summary>
    /// Makes room instances with enemies and interactables
    /// uses enemy factory internally for enemy creation
    /// </summary>
    public class RoomFactory
    {
        private readonly IContentLoader _loader;
        private readonly EnemyFactory _enemyFactory;

        public RoomFactory(IContentLoader loader)
        {
            _loader = loader;
            _enemyFactory = new EnemyFactory(loader);
        }

        /// <summary>
        /// create a fully configured room by ID
        /// </summary>
        public Room CreateRoom(string roomId)
        {
            var data = _loader.GetRoomData(roomId);
            if (data == null)
            {
                Console.WriteLine($"[RoomFactory] Unknown room: {roomId}");
                return null;
            }

            // Build normal enemies
            var normalEnemies = data.Enemies
                .Select(id => _enemyFactory.CreateEnemy(id) as NormalEnemy)
                .Where(e => e != null)
                .ToList();

            // Build boss if present
            BossEnemy boss = null;
            if (!string.IsNullOrEmpty(data.Boss))
                boss = _enemyFactory.CreateEnemy(data.Boss) as BossEnemy;

            return new Room(roomId, normalEnemies, boss, _loader);
        }

        /// <summary>
        /// Builds the entire map from rooms.json.
        /// Creates all rooms and connects them based on connections field.
        /// Call this instead of creating rooms manually
        /// </summary>

        public Map BuildMap(string startingRoomId)
        {
            var map = new Map();
            var allRoomIds = GetAllRoomIds();

            // 1. create all rooms
            foreach (var roomId in allRoomIds)
            {
                var room = CreateRoom(roomId);
                if (room != null)
                    map.AddRoom(room);
            }

            // 2. connect rooms
            var connected = new HashSet<string>();
            foreach (var roomId in allRoomIds)
            {
                var data = _loader.GetRoomData(roomId);
                if (data?.Connections == null) continue;

                foreach (var neighbourId in data.Connections)
                {
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

        private List<string> GetAllRoomIds()
        {
            // future: IContentLoader could expose a GetAllRoomIds() method
            // for now hardcode known room IDs
            return new List<string>
            {
                "entrance_hall",
                "kitchen",
                "child_bedroom",
                "library"
            };
        }
    }
}