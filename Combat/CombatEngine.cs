using System;
using HauntedMansion.Entities;

namespace HauntedMansion.Combat
{
    public enum AttackType { Physical, Magical }

    // abstraction for random number generation,
    // using this instead of a hardcoded new random easier for testing
    public interface IRngProvider
    {
        double NextDouble();
    }
    
    public class DefaultRngProvider : IRngProvider
    {
        private readonly Random _rng = new();
        public double NextDouble() => _rng.NextDouble();
    }

    
    // only for battle calculations
    public class CombatEngine
    {
        private readonly IRngProvider _rng;

        // defaults to random rng 
        public CombatEngine(IRngProvider rng = null)
        {
            _rng = rng ?? new DefaultRngProvider();
        }

        // probability to hit at all
        public float CalculateHitChance(Entity attacker, Entity defender, BodyPart targetPart = null)
        {
            var atkStats = attacker.GetEffectiveStats();
            var defStats = defender.GetEffectiveStats();

            // Base accuracy divided by 100
            // Evasion factor comparing defender speed vs attacker speed
            // Multiplier if aiming for a harder-to-hit body part
            float baseHitChance = atkStats.Accuracy / 100f;     
            float evasionFactor = 100f / (100f + Math.Max(0, defStats.Speed - atkStats.Speed));
            float partModifier = targetPart?.HitChanceModifier ?? 1.0f;
            
            // between 0.05 and 0.95
            return Math.Clamp(baseHitChance * partModifier * evasionFactor, 0.05f, 0.95f);
        }

        // Calculates damage based on stats
        public int CalculateDamage(Entity attacker, Entity defender, AttackType type, BodyPart targetPart = null)
        {
            var atkStats = attacker.GetEffectiveStats();
            var defStats = defender.GetEffectiveStats();

            // correct defense stat based on attack type
            int rawDamage = type switch
            {
                AttackType.Physical => (int)(atkStats.Attack * (100f / (100f + defStats.Defence))),
                AttackType.Magical => (int)(atkStats.Magic * (100f / (100f + defStats.Magic))),
                _ => 1
            };
            rawDamage = Math.Max(1, rawDamage); // min 1 dmg
            
            // doby part multiplier
            if (targetPart != null) return (int)Math.Round(rawDamage * targetPart.DamageMultiplier);
            return rawDamage;
        }

        // main method called by actions
        // rolls the dice, checks for a hit, applies damage,
        // triggers body part events, and returns a CombatResult object
        public CombatResult ExecuteAttack(Entity attacker, Entity defender, AttackType type, BodyPart targetPart = null)
        {
            float hitChance = CalculateHitChance(attacker, defender, targetPart);
            double roll = _rng.NextDouble();

            // miss
            if (roll > hitChance)
            {
                return new CombatResult { WasHit = false, DamageDealt = 0, Message = $"{attacker.Name} missed!" };
            }

            // hit
            int damage = CalculateDamage(attacker, defender, type, targetPart);
            defender.TakeDamage(damage);
            
            // trigger potential extra effect of body part
            if (targetPart != null && defender is Enemy enemyTarget)
                targetPart.OnHit(enemyTarget);

            string partInfo = targetPart != null ? $"({targetPart.PartType})" : "";

            return new CombatResult
            {
                WasHit = true,
                DamageDealt = damage,
                Message = $"{attacker.Name} hit{partInfo} for {damage} damage!"
            };
        }
    }
}