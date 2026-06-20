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

        public void Interact(Player player)
        {
            //future: DialogueEngine.StartConversation(this)
            Console.WriteLine($"{Name}: ...");
        }

        public string GetDescription() => _description;
        public string GetStartingNode() => _startingNodeId;
    }
}