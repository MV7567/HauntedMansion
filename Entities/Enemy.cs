using System.Collections.Generic;
using System.Linq;
using HauntedMansion.Core;
using HauntedMansion.Combat;
using HauntedMansion.Combat.Interfaces;

namespace HauntedMansion.Entities;

/// <summary>
/// Base class for all enemies
/// </summary>
public abstract class Enemy : Entity
{
    private List<BodyPart> BodyParts { get; init; }
    private ICombatState CurrentState { get; set; }
    private IEnemyAi AI { get; init; }

    protected Enemy(string name, CharacterStats baseStats, List<BodyPart> bodyParts, IEnemyAi ai)
            : base(name, baseStats)
        {
            BodyParts = bodyParts ?? new List<BodyPart>();
            AI = ai;
        }

    public BodyPart GetBodyPart(BodyPartType type)
    {
        return BodyParts.FirstOrDefault(bp => bp.PartType == type);
    }

    public void SetState(ICombatState state)
    {
        // future: finish combat states
        //CurrentState?.OnExit(this);
        CurrentState = state;
        //CurrentState?.OnEnter(this);
    }

    public IAction GetAction(CombatContext context)
    {
        // future: add IAction interface
        //IAction stateAction = CurrentState?.Execute(this, context);
        //if (stateAction != null) return stateAction;
        //return AI?.ChooseAction(this, context);
    }

    public override CharacterStats GetEffectiveStats()
    {
        // ememies return their base stats, modifiers are applied directly
        return Stats;
    }
}