using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Entities;

namespace HauntedMansion.Combat.Actions
{
    /// <summary>
    /// Basic physical attack
    /// the attacker, the engine that performs the math, the attack type
    /// </summary>
    public class AttackAction : IAction
    {
        private readonly Enemy _attacker;
        private readonly CombatEngine _engine;
        private readonly AttackType _type;

        // Dependencies so the action has everything to resolve
        public AttackAction(Enemy attacker, CombatEngine engine,
            AttackType type = AttackType.Physical)
        {
            _attacker = attacker;
            _engine = engine;
            _type = type;
        }

        // delegates the actual math to CombatEngine
        public CombatResult? Execute(CombatContext? context)
        {
            if (context == null) return null;
            return _engine.ExecuteAttack(_attacker, context.Player, _type);
        }
    }
}