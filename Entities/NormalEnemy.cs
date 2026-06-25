using System.Collections.Generic;
using HauntedMansion.Core;
using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Dialogue;

namespace HauntedMansion.Entities;

/// <summary>
/// Regular enemies that appear through random encounters
/// excluded from the rooms enemy pool once defeated
/// (rooms become empty over time)
/// </summary>
public class NormalEnemy : Enemy, IDialoguable
{
    public float EncounterWeight { get; init; }
    public bool IsDefeated { get; private set; }
    private string StartingNodeID { get; init; }

    public NormalEnemy(string name, CharacterStats baseStats, List<BodyPart> bodyParts, 
        IEnemyAi ai, float weight, string startingNodeId)
        : base(name, baseStats, bodyParts, ai)
    {
        EncounterWeight = weight;
        StartingNodeID = startingNodeId;
        IsDefeated = false;
    }

    public void MarkDefeated()
    {
        IsDefeated = true;
    }
    
    // IDialoguable implementation for basic enemy taunts
    public string GetStartingNode() => StartingNodeID;
    public string GetBaseTreeId() => StartingNodeID;
}