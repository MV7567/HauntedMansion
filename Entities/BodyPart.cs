namespace HauntedMansion.Entities
{
    /// <summary>
    /// Targetable part of an enemy
    /// DamageMultiplier and HitChanceModifier create tradeoffs
    /// CombatEngine calls 'OnHit()' when this part takes damage
    /// allows to implement future mechanics (like dropping a weapon)
    /// </summary>
    public enum BodyPartType { Head, Torso, RightArm, LeftArm, Legs }

    public class BodyPart
    {
        public BodyPartType PartType { get; init; }
        // Modifies the CombatEngine's hit chance calculation
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

        // Hook called by the CombatEngine upon a successful hit
        public void OnHit(Enemy target)
        {
            // future: part-specific special effects
        }
    }
}