using HauntedMansion.Core;
using HauntedMansion.Combat;
using HauntedMansion.Combat.Interfaces;

namespace HauntedMansion.Entities;

public abstract class Enemy : Entity
{
    private List<BodyPart> BodyParts { get; init; }
    private IEnemyAi AI { get; init; }
    
    public bool IsSparable { get; set; } 
    
    public int StateAttackMod { get; private set; }
    public int StateDefenceMod { get; private set; }
    public int StateSpeedMod { get; private set; }
    public int StateMagicMod { get; private set; }
    public int StateAccuracyMod { get; private set; }

    protected Enemy(string name, CharacterStats baseStats, List<BodyPart> bodyParts, IEnemyAi ai)
            : base(name, baseStats)
    {
        BodyParts = bodyParts ?? new List<BodyPart>();
        AI = ai;
    }

    public void ApplyStateMod(int attack = 0, int defence = 0, int speed = 0, int magic = 0, int accuracy = 0)
    {
        StateAttackMod += attack;
        StateDefenceMod += defence;
        StateSpeedMod += speed;
        StateMagicMod += magic;
        StateAccuracyMod += accuracy;
    }

    public void ResetStateMod()
    {
        StateAttackMod = StateDefenceMod = StateSpeedMod = StateMagicMod = StateAccuracyMod = 0;
    }
    
    public override CharacterStats GetEffectiveStats()
    {
        return new CharacterStats(
            Stats.Attack + StateAttackMod,
            Stats.Defence + StateDefenceMod,
            Stats.Magic + StateMagicMod,
            Stats.Speed + StateSpeedMod,
            Stats.Accuracy + StateAccuracyMod,
            Stats.MaxHP
        );
    }
    
    public BodyPart GetBodyPart(BodyPartType type) => BodyParts.FirstOrDefault(bp => bp.PartType == type);

    // enemy only uses ai strategy
    public IAction GetAction(CombatContext context) => AI?.ChooseAction(this, context);
}