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
            return roomId switch
            {
                "kitchen" => CreateKitchen(),
                "child_bedroom" => CreateChildBedroom(),
                "library" => CreateLibrary(),
                "entrance_hall" => CreateEntranceHall(),
                _ => null
            };
        }

        private Room CreateKitchen()
        {
            var enemies = new List<NormalEnemy>();
            // future: add normal enemies when AI is implemented

            var boss = _enemyFactory.CreateEnemy("rat_chef") as BossEnemy;

            return new Room(
                roomId: "kitchen",
                normalEnemies: enemies,
                BossEnemy: boss,
                loader: _loader
            );
        }

        private Room CreateChildBedroom()
        {
            var enemies = new List<NormalEnemy>();
            
            var boss = _enemyFactory.CreateEnemy("ghost_child") as BossEnemy

            return new Room(
                roomId: "child_bedroom",
                normalEnemies: enemies,
                BossEnemy: boss,
                loader: _loader
            );
        }
        
        private Room CreateLibrary()
        {
            var enemies = new List<NormalEnemy>
            {
                _enemyFactory.CreateEnemy("living_armor") as NormalEnemy
            };

            return new Room(
                roomId: "library",
                normalEnemies: enemies,
                bossEnemy: null,
                loader: _loader
            );
        }
        
        private Room CreateEntranceHall()
        {
            return new Room(
                roomId: "entrance_hall",
                normalEnemies: new List<NormalEnemy>(),
                bossEnemy: null,
                loader: _loader
            );
        }

    }
}