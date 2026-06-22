using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Combat.Actions;
using HauntedMansion.Entities;

namespace HauntedMansion.Combat.States
{
    /// <summary>
    /// Enemy is at low HP and/or missing body parts
    /// </summary>
    public class WeakenedState : ICombatState
    {
        private const int AttackPenalty = -3;
        private const int SpeedPenalty = -3;

        public string OnEnter(Enemy enemy)
        {
            enemy.ApplyStateMod(attack: AttackPenalty, speed: SpeedPenalty);
            return $"{enemy.Name} looks weakened...";
        }

        public IAction Execute(Enemy enemy, CombatContext context)
        {
            // Return null to let AI decide
            // AI may still attack but with reduced stats
            // Future: return new DefendAction() or FleeAction();
            return null;
        }

        public string OnExit(Enemy enemy)
        {
            enemy.ResetStateMod();
            return string.Empty;
        }
    }
}