using System;
using HauntedMansion.Entities;

namespace HauntedMansion.Combat
{
    public enum AttackType { Physical, Magical }

    public interface IRngProvider
    {
        double NextDouble();
    }

    public class DefaultRngProvider : IRngProvider
    {
        private readonly Random _rng = new();
        public double NextDouble() => _rng.NextDouble();
    }

    public class CombatEngine
    {
        private readonly IRngProvider _rng;

        public CombatEngine(IRngProvider rng = null)
        {
            _rng = rng ?? new DefaultRngProvider();
        }

        public float CalculateHitChance(Entity attacker, Entity defender, BodyPart targetPart = null)
        {
            var atkStats = attacker.GetEffectiveStats();
            var defStats = defender.GetEffectiveStats();

            float baseHitChance = atkStats.Accuracy / 100f;     
            float evasionFactor = 100f / (100f + Math.Max(0, defStats.Speed - atkStats.Speed));
            float partModifier = targetPart?.HitChanceModifier ?? 1.0f;
            
            return Math.Clamp(baseHitChance * partModifier * evasionFactor, 0.05f, 0.95f);
        }

        public int CalculateDamage(Entity attacker, Entity defender, AttackType type, BodyPart targetPart = null)
        {
            var atkStats = attacker.GetEffectiveStats();
            var defStats = defender.GetEffectiveStats();

            int rawDamage = type switch
            {
                AttackType.Physical => (int)(atkStats.Attack * (100f / (100f + defStats.Defence))),
                AttackType.Magical => (int)(atkStats.Magic * (100f / (100f + defStats.Magic))),
                _ => 1
            };
            rawDamage = Math.Max(1, rawDamage);
            
            if (targetPart != null) return (int)Math.Round(rawDamage * targetPart.DamageMultiplier);
            return rawDamage;
        }

        public CombatResult ExecuteAttack(Entity attacker, Entity defender, AttackType type, BodyPart targetPart = null)
        {
            float hitChance = CalculateHitChance(attacker, defender, targetPart);
            double roll = _rng.NextDouble();

            if (roll > hitChance)
            {
                return new CombatResult { WasHit = false, DamageDealt = 0, Message = $"{attacker.Name} missed!" };
            }

            int damage = CalculateDamage(attacker, defender, type, targetPart);
            defender.TakeDamage(damage);
            
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