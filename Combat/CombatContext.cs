using System.Collections.Generic;
using HauntedMansion.Entities;

namespace HauntedMansion.Combat
{
    /// <summary>
    /// Data container in the combat system
    /// AI and states have information about the battle situation
    /// </summary>
    public class CombatContext
    {
        public Player Player { get; init; }
        public List<Enemy> Enemies { get; init; }
        public int TurnNumber { get; set; }

        public CombatContext(Player player, List<Enemy> enemies)
        {
            Player = player;
            Enemies = enemies ?? new List<Enemy>();
            TurnNumber = 1;
        }
    }
}