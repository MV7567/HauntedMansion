namespace HauntedMansion.Dialogue
{
    /// <summary>
    /// holds a complete dialogue tree from json
    /// </summary>
    public class DialogueTree
    {
        private readonly Dictionary<string, DialogueNode> _nodes = new();

        public void AddNode(DialogueNode node)
        {
            _nodes[node.NodeID] = node;
        }
        /// <summary>
        /// returns node by ID (or null)
        /// </summary>
        public DialogueNode GetNode(string nodeId)
        {
            return _nodes.TryGetValue(nodeId, out var node) ? node : null;
        }
    }
}