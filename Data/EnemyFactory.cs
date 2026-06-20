using HauntedMansion.Core;
using HauntedMansion.Entities;

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
            return enemyId switch
            {
                "ghost_child" => CreateGhostChild(),
                "rat_chef" => CreateRatChef(),
                "living_armor" => CreateLivingArmor(),
                _ => null
            };
        }

        private BossEnemy CreateGhostChild()
        {
            var stats = new CharacterStats(
                attack: 8, defence: 3, magic: 15,
                speed: 12, accuracy: 70, maxHP: 40
            );

            var bodyParts = new List<BodyPart>
            {
                new BodyPart(BodyPartType.Head, hitMod: 0.6f, dmgMult: 1.8f),
                new BodyPart(BodyPartType.Torso, hitMod: 1.0f, dmgMult: 1.0f),
                new BodyPart(BodyPartType.LeftArm, hitMod: 0.8f, dmgMult: 0.7f),
                new BodyPart(BodyPartType.RightArm, hitMod: 0.8f, dmgMult: 0.7f),
                new BodyPart(BodyPartType.Legs, hitMod: 0.9f, dmgMult: 0.8f)
            };
            
            return new BossEnemy(
                name: "Ghost Child",
                baseStats: stats,
                bodyParts: bodyParts,
                ai: null,
                startingNodeId: "ghost_child"
            );
        }

        private BossEnemy CreateRatChef()
        {
            var stats = new CharacterStats(
                attack: 18, defence: 8, magic: 5,
                speed: 10, accuracy: 75, maxHP: 80
            );

            var bodyParts = new List<BodyPart>
            {
                new BodyPart(BodyPartType.Head,   hitMod: 0.5f, dmgMult: 2.0f),
                new BodyPart(BodyPartType.Torso,  hitMod: 1.0f, dmgMult: 1.0f),
                new BodyPart(BodyPartType.LeftArm,  hitMod: 0.8f, dmgMult: 0.8f),
                new BodyPart(BodyPartType.RightArm, hitMod: 0.8f, dmgMult: 0.8f),
                new BodyPart(BodyPartType.Legs,   hitMod: 0.9f, dmgMult: 0.7f)
            };

            return new BossEnemy(
                name: "Rat Chef",
                baseStats: stats,
                bodyParts: bodyParts,
                ai: null,
                startingNodeId: "rat_chef"
            );
        }
        
        private NormalEnemy CreateLivingArmor()
        {
            var stats = new CharacterStats(
                attack: 20, defence: 25, magic: 0,
                speed: 5, accuracy: 65, maxHP: 60
            );

            var bodyParts = new List<BodyPart>
            {
                new BodyPart(BodyPartType.Head,   hitMod: 0.5f, dmgMult: 1.5f),
                new BodyPart(BodyPartType.Torso,  hitMod: 1.0f, dmgMult: 1.0f),
                new BodyPart(BodyPartType.LeftArm,  hitMod: 0.8f, dmgMult: 0.9f),
                new BodyPart(BodyPartType.RightArm, hitMod: 0.8f, dmgMult: 0.9f),
                new BodyPart(BodyPartType.Legs,   hitMod: 0.9f, dmgMult: 0.8f)
            };

            return new NormalEnemy(
                name: "Living Armor",
                baseStats: stats,
                bodyParts: bodyParts,
                ai: null,
                weight: 0.7f,
                startingNodeId: "living_armor"
            );
        }

    }
}