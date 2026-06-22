using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Entities;

namespace HauntedMansion.Combat.Actions
{
    /// <summary>
    /// Basic physical attack
    /// </summary>
    public class AttackAction : IAction
    {
        private readonly Enemy _attacker;
        private readonly CombatEngine _engine;
        private readonly AttackType _type;

        public AttackAction(Enemy attacker, CombatEngine engine,
            AttackType type = AttackType.Physical)
        {
            _attacker = attacker;
            _engine = engine;
            _type = type;
        }

        public CombatResult? Execute(CombatContext? context)
        {
            if (context == null) return null;
            return _engine.ExecuteAttack(_attacker, context.Player, _type);
        }
    }
}