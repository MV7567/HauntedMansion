using System.Collections.Generic;
using System.Linq;
using HauntedMansion.Core;
using HauntedMansion.Combat;
using HauntedMansion.Combat.Actions;
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
    
    // Modified by combat states via ApplyStateMod()
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
        StateAttackMod = 0;
        StateDefenceMod = 0;
        StateSpeedMod = 0;
        StateMagicMod = 0;
        StateAccuracyMod = 0;
    }
    
    public override CharacterStats GetEffectiveStats()
    {
        return new CharacterStats(
            Stats.Attack   + StateAttackMod,
            Stats.Defence  + StateDefenceMod,
            Stats.Magic + StateMagicMod,
            Stats.Speed    + StateSpeedMod,
            Stats.Accuracy + StateAccuracyMod,
            Stats.MaxHP
        );
    }

    
    
    public BodyPart GetBodyPart(BodyPartType type)
    {
        return BodyParts.FirstOrDefault(bp => bp.PartType == type);
    }

    public void SetState(ICombatState state)
    {
        CurrentState?.OnExit(this);
        CurrentState = state;
        CurrentState?.OnEnter(this);
    }
    
    // New version that returns messages:
    public (string exitMsg, string enterMsg) SetStateWithMessages(ICombatState state)
    {
        string exitMsg = CurrentState?.OnExit(this) ?? string.Empty;
        CurrentState = state;
        string enterMsg = CurrentState?.OnEnter(this) ?? string.Empty;
        return (exitMsg, enterMsg);
    }

    public IAction GetAction(CombatContext context)
    {
        // State gets priority - e.g. SparableState returns null = no attack
        IAction stateAction = CurrentState?.Execute(this, context);
        if (stateAction != null) return stateAction;
        
        // Fall back to AI if state returns null
        return AI?.ChooseAction(this, context);
    }
    
    /// <summary>
    /// Returns true if enemy is in SparableState.
    /// Avoids calling GetAction() as a side-effect check.
    /// </summary>
    public bool IsSparable(CombatContext context)
    {
        return GetAction(context) is IdleAction;
    }
}