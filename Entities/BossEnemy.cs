using HauntedMansion.Core;
using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Dialogue;
using HauntedMansion.Interactions;

namespace HauntedMansion.Entities
{
    /// <summary>
    /// A unique, story-relevant enemy triggered by entering a room
    /// Can turn into NPC after being appeased
    /// </summary>
    public class BossEnemy : Enemy, IDialoguable, IInteractable
    {
        public bool IsAppeared { get; private set; }
        public bool PostBattleNPC { get; private set; }
        private string StartingNodeID { get; init; }
        private string PostBattleNodeID { get; init; }
        private string BaseTreeID { get; init; }
        

        public BossEnemy(string name, CharacterStats baseStats, List<BodyPart> bodyParts,
            IEnemyAi ai, string startingNodeId, string postBattleNodeId, string baseTreeId)
            : base(name, baseStats, bodyParts, ai)
        {
            IsAppeared = false;
            PostBattleNPC = false;
            StartingNodeID = startingNodeId;
            PostBattleNodeID = postBattleNodeId;
            BaseTreeID = baseTreeId;
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

        // IDialoguable implementation changes based on whether the battle is over
        public string GetStartingNode() => PostBattleNPC ? PostBattleNodeID : StartingNodeID;
        public string GetBaseTreeId() => BaseTreeID;
        
        public string Interact(Player player)
        {
            return string.Empty;
        }
        
        public string GetDescription() => $"{Name} stands here peacefully.";
    }
}