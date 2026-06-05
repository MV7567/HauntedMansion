using HauntedMansion.Entities;

namespace HauntedMansion.Combat
{
    public enum AttackType { Physical, Magical }
    /// <summary>
    /// Combat calculations.
    /// </summary>
    public class CombatEngine
    {
        private readonly Random _rng = new Random();

        // Calculates hit chance. BodyPart modifier applied if targetPart provided.
        // Result clamped between 5% and 95%.
        public float CalculateHitChance(Entity attacker, Entity defender, BodyPart targetPart = null)
        {
            var atkStats = attacker.GetEffectiveStats();
            var defStats = defender.GetEffectiveStats();

            // base accuracy
            float baseHitChance = atkStats.Accuracy / 100f;     
            
            // Speed evasion: same formula as damage
            float evasionFactor = 100f / (100f + Math.Max(0, defStats.Speed - atkStats.Speed));
            
            // Body part modifier if targeting specific part
            float partModifier = targetPart?.HitChanceModifier ?? 1.0f;
            
            return Math.Clamp(baseHitChance * partModifier * evasionFactor, 0.05f, 0.95f);
        }

        // Calculates damage using RPG scaling formula.
        public int CalculateDamage(Entity attacker, Entity defender,
            AttackType type, BodyPart targetPart = null)
        {
            var atkStats = attacker.GetEffectiveStats();
            var defStats = defender.GetEffectiveStats();

            int rawDamage = type switch
            {
                AttackType.Physical => (int)(atkStats.Attack *
                                             (100f / (100f + defStats.Defence))),
                AttackType.Magical => (int)(atkStats.Magic *
                                            (100f / (100f + defStats.Magic))),
                _ => 1
            };
            rawDamage = Math.Max(1, rawDamage);
            
            if (targetPart != null)
                return (int)Math.Round(rawDamage * targetPart.DamageMultiplier);

            return rawDamage;
        }

        // Executes a full attack: rolls hit chance, calculates damage,
        // applies damage, triggers OnHit() for body part special effects.
        // targetPart is optional - enemy attacks don't target specific parts.
        public CombatResult ExecuteAttack(Entity attacker, Entity defender,
                                          AttackType type, BodyPart targetPart = null)
        {
            float hitChance = CalculateHitChance(attacker, defender, targetPart);
            double roll = _rng.NextDouble();        // rolls between 0.0 and 1.0

            if (roll < hitChance)
            {
                return new CombatResult
                {
                    WasHit = false,
                    DamageDealt = 0,
                    Message = $"{attacker.Name} missed!"
                };
            }

            int damage = CalculateDamage(attacker, defender, type, targetPart);
            defender.TakeDamage(damage);
            
            // trigger special effect only for targeted attacks on enemies
            if (targetPart != null && defender is Enemy enemyTarget)
                targetPart.OnHit(enemyTarget);

            string partInfo = targetPart != null
                ? $"({targetPart.PartType})"
                : "";

            return new CombatResult
            {
                WasHit = true,
                DamageDealt = damage,
                Message = $"{attacker.Name} hit{partInfo} for{damage} damage!"
            };
        }
    }
}