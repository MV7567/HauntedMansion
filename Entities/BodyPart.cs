namespace HauntedMansion.Entities
{
    /// <summary>
    /// Targetable part of an enemy
    /// DamageMultiplier and HitChanceModifier create tradeoffs 
    /// </summary>
    public enum BodyPartType { Head, Torso, RightArm, LeftArm, Legs }

    public class BodyPart
    {
        public BodyPartType PartType { get; init; }
        public float HitChanceModifier { get; init; }
        public float DamageMultiplier { get; init; }
        public bool IsDisabled { get; private set; }

        public BodyPart(BodyPartType type, float hitMod, float dmgMult)
        {
            PartType = type;
            HitChanceModifier = hitMod;
            DamageMultiplier = dmgMult;
            IsDisabled = false;
        }

        public void Disable()
        {
            IsDisabled = true;
        }

        public void OnHit(Enemy target)
        {
            // future: part-specific special effects
        }
    }
}