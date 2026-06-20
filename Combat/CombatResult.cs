namespace HauntedMansion.Combat
{
    /// <summary>
    /// Data transfer object. Holds the result of an attack.
    /// </summary>
    public class CombatResult
    {
        public bool WasHit { get; init; }
        public int DamageDealt { get; init; }

        public string Message { get; init; }
    }
}