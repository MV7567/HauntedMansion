using System.Collections.Generic;
using HauntedMansion.Core;
using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Dialogue;

namespace HauntedMansion.Entities
{
    /// <summary>
    /// A unique, story-relevant enemy triggered by entering a room
    /// Can turn into NPC after being appeased
    /// </summary>
    public class BossEnemy : Enemy, IDialoguable
    {
        public bool IsAppeared { get; private set; }
        public bool PostBattleNPC { get; private set; }
        private string StartingNodeID { get; init; }

        public BossEnemy(string name, CharacterStats baseStats, List<BodyPart> bodyParts,
            IEnemyAi ai, string startingNodeId)
            : base(name, baseStats, bodyParts, ai)
        {
            IsAppeared = false;
            PostBattleNPC = false;
            StartingNodeID = startingNodeId;
        }

        public void TriggerBattle(Player player)
        {
            if (IsAppeared) return;
            IsAppeared = true;
            // additional logic in GameLoop/CombatGameState
        }

        public void BecomeNPC()
        {
            PostBattleNPC = true;
            // Removes combat capabilities and leaves only IInteractable/IDialoguable features
        }

        public string GetStartingNode() => StartingNodeID;
    }
}