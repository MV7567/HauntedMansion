namespace HauntedMansion.Dialogue
{
    /// <summary>
    /// single node in dialogue tree
    /// empty choice is a terminal node
    /// loaded from json
    /// </summary>
    public class DialogueNode
    {
        public string NodeID { get; init; }
        public string Text { get; init; }
        
        // outgoing options
        public List<DialogueChoice> Choices { get; init; } = new();

        // convo ends
        public bool IsTerminal => Choices.Count == 0;
    }
}