using HauntedMansion.Dialogue;
using HauntedMansion.Entities;

namespace HauntedMansion.Interactions
{
    /// <summary>
    /// Frienly NPC with dialogue tree via IDialoguable
    /// no combat behavior
    /// Interact() starts the converasation (Dialogue engine)
    /// </summary>
    public class NormalNPC : IInteractable, IDialoguable
    {
        public string Name { get; init; }
        private readonly string _startingNodeId;
        private readonly string _description;

        public NormalNPC(string name, string description, string startingNodeId)
        {
            Name = name;
            _description = description;
            _startingNodeId = startingNodeId;
        }

        public string Interact(Player player)
        {
            // Returns empty - ExplorationGameState checks if
            // interactable is IDialoguable and starts DialogueEngine
            return string.Empty;
        }

        public string GetDescription() => _description;
        public string GetStartingNode() => _startingNodeId;
        public string GetBaseTreeId() => _startingNodeId;
    }
}