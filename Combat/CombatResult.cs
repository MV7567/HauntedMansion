namespace HauntedMansion.Combat
{
    /// <summary>
    /// Data transfer object. Holds the result of an attack.
    /// CombatEngine calculates the damage and returns this
    /// The UI receives this and decides how to display it
    /// </summary>
    public class CombatResult
    {
        // hit or miss
        public bool WasHit { get; init; }
        // amount of dmg
        public int DamageDealt { get; init; }

        //string describing what happened
        public string Message { get; init; }
    }
}