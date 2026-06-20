using HauntedMansion.Data;
using HauntedMansion.Entities;
using HauntedMansion.Interactions;

namespace HauntedMansion.World
{
    /// <summary>
    /// Room implementation
    /// owns normal and boss enemies
    /// aggregates IInteractable objects
    /// </summary>
    public class Room : IRoom
    {
        private readonly string _roomId;
        private readonly IContentLoader _loader;
        
        // enemies created and owned by the room
        private readonly List<NormalEnemy> _normalEnemies;
        private readonly BossEnemy _bossEnemy; //null if room has no boss

        private readonly List<IInteractable> _interactables = new();

        private bool _isCleared;
        private readonly Random _rng = new();

        public Room(string roomId, List<NormalEnemy> normalEnemies,
            BossEnemy bossEnemy, IContentLoader loader)
        {
            _roomId = roomId;
            _normalEnemies = normalEnemies ?? new List<NormalEnemy>();
            _bossEnemy = bossEnemy;
            _loader = loader;
        }

        public string GetRoomID() => _roomId;

        public List<IInteractable> GetInteractables() => _interactables;

        /// <summary>
        /// Returns all living enemies in this room
        /// </summary>
        public List<Enemy> GetEnemies()
        {
            var enemies = new List<Enemy>();
            
            enemies.AddRange(_normalEnemies.Where(e => !e.IsDefeated));
            
            if (_bossEnemy != null && _bossEnemy.IsAlive() && !_bossEnemy.PostBattleNPC)
                enemies.Add(_bossEnemy);

            return enemies;
        }

        /// <summary>
        /// Called by Map.MoveToRoom() every time player enters
        /// </summary>
        public void OnEnter(Player player)
        {
            // display room description form json
            string description = _loader.GetRoomDescription(_roomId);
            Console.WriteLine($"\n--- {_roomId.ToUpper()} ---");
            Console.WriteLine(description);
            
            // trigger boss if not yet encountered
            _bossEnemy?.TriggerBattle(player);

            UpdateClearedStatus();
        }

        /// <summary>
        /// called on by player during exploring
        /// rolls against undefeated NormalEnemy encounter weights
        /// returns true if triggered an encounter
        /// </summary>
        public bool TryTriggerEncounter(Player player)
        {
            var activeEnemies = _normalEnemies.Where(e => !e.IsDefeated).ToList();
            if (activeEnemies.Count == 0) return false;
            
            // base 30% chance of encounter per step
            float baseChance = 0.3f;
            if (_rng.NextDouble() > baseChance) return false;
            
            // Weight-based selection from active enemies
            float totalWeight = activeEnemies.Sum(e => e.EncounterWeight);
            float roll = (float)_rng.NextDouble() * totalWeight;

            float cumulative = 0f;
            foreach (var enemy in activeEnemies)
            {
                cumulative += enemy.EncounterWeight;
                if (roll <= cumulative)
                {
                    Console.WriteLine($"\nA {enemy.Name} appears!");
                }
            }

            return false;
        }
        
        /// <summary>
        /// adds an interactable object to the room
        /// </summary>
        public void AddInteractable(IInteractable interactable)
        {
            _interactables.Add(interactable);
        }

        /// <summary>
        /// checks if enemies are defeated or spared
        /// used for ending calculation
        /// </summary>
        private void UpdateClearedStatus()
        {
            bool normalCleared = _normalEnemies.All(e => e.IsDefeated);
            bool bossCleared = _bossEnemy == null || !_bossEnemy.IsAlive() || _bossEnemy.PostBattleNPC;

            _isCleared = normalCleared && bossCleared;
        }
        public bool IsCleared => _isCleared;
    }
}