using HauntedMansion.Core;
using HauntedMansion.Entities;
using HauntedMansion.Combat.AI;

namespace HauntedMansion.Data
{
    /// <summary>
    /// Builds enemy instances from json data
    /// depends on IContentLoader not JsonContentLoader directly
    /// </summary>
    public class EnemyFactory
    {
        private readonly IContentLoader _loader;

        public EnemyFactory(IContentLoader loader)
        {
            _loader = loader;
        }

        /// <summary>
        /// Creates fully configured enemy by ID
        /// enemyID matches keys in enemies.json
        /// return null if enemyID not recognised
        /// </summary>
        public Enemy CreateEnemy(string enemyId)
        {
            var data = _loader.GetEnemyData(enemyId);
            if (data == null)
            {
                Console.WriteLine($"[EnemyFactory] Unknown enemy: {enemyId}");
                return null;
            }
            
            var stats = new CharacterStats(
                data.Stats.Attack,
                data.Stats.Defence,
                data.Stats.Magic,
                data.Stats.Speed,
                data.Stats.Accuracy,
                data.Stats.MaxHP
            );
            
            var bodyParts = data.BodyParts.Select(bp => new BodyPart(
                ParseBodyPartType(bp.Type),
                bp.HitMod,
                bp.DmgMult
            )).ToList();
            
            var ai = new BasicEnemyAI();
            
            return data.Type == "boss"
                ? new BossEnemy(data.Name, stats, bodyParts, ai, data.StartingNodeId)
                : new NormalEnemy(data.Name, stats, bodyParts, ai,
                    data.EncounterWeight, data.StartingNodeId);
        }
        
        private BodyPartType ParseBodyPartType(string type)
        {
            return type switch
            {
                "Head"     => BodyPartType.Head,
                "Torso"    => BodyPartType.Torso,
                "LeftArm"  => BodyPartType.LeftArm,
                "RightArm" => BodyPartType.RightArm,
                "Legs"     => BodyPartType.Legs,
                _ => BodyPartType.Torso
            };
        }
    }
}