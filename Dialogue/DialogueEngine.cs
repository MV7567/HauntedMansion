using HauntedMansion.Combat;
using HauntedMansion.Data;
using HauntedMansion.Entities;

namespace HauntedMansion.Dialogue
{
    /// <summary>
    /// Manages conversations
    /// executes ICommand on each choice selection
    /// </summary>
    public class DialogueEngine
    {
        private readonly IContentLoader _loader;
        private DialogueNode? _currentNode;
        private DialogueTree? _currentTree;

        public bool IsActive { get; private set; }

        // Current node exposed for renderer
        public DialogueNode? CurrentNode => _currentNode;
        
        public DialogueEngine(IContentLoader loader)
        {
            _loader = loader;
        }

        /// <summary>
        /// loads dialogue tree for entity and sets the first node 
        /// </summary>
        /// <param name="entity"></param>
        public void StartConversation(IDialoguable entity)
        {
            _currentTree = LoadTree(entity.GetStartingNode());

            if (_currentTree == null)
            {
                IsActive = false;
                return;
            }

            _currentNode = _currentTree.GetNode(entity.GetStartingNode());
            IsActive = _currentNode != null;
        }

        /// <summary>
        /// Processes choice. Returns message from action if any.
        /// Caller uses CurrentNode for display via IRenderer.
        /// </summary>
        public string SelectChoice(int choiceIndex, Player player,
            CombatContext? context = null)
        {
            if (!IsActive || _currentNode == null) return string.Empty;

            if (choiceIndex < 0 || choiceIndex >= _currentNode.Choices.Count)
                return "Invalid choice.";

            var choice = _currentNode.Choices[choiceIndex];

            // Execute dialogue effect if present
            string actionMessage = string.Empty;
            if (choice.Action != null)
                actionMessage = choice.Action.Execute(context) ?? string.Empty;

            // Move to next node
            if (string.IsNullOrEmpty(choice.NextNodeID))
            {
                EndConversation();
                return actionMessage;
            }

            _currentNode = _currentTree?.GetNode(choice.NextNodeID);

            if (_currentNode == null || _currentNode.IsTerminal)
            {
                IsActive = false;
                return actionMessage;
            }

            return actionMessage;
        }

        /// <summary>
        /// returns current choices for irenderer to display
        /// </summary>
        /// <returns></returns>
        public List<DialogueChoice> GetCurrentChoices() =>
            _currentNode?.Choices ?? new List<DialogueChoice>();

        public void EndConversation()
        {
            IsActive = false;
            _currentNode = null;
            _currentTree = null;
        }

        private DialogueTree? LoadTree(string entityId)
        {
            var lines = _loader.GetDialogueLines(entityId);
            if (lines == null || lines.Count == 0) return null;
            
            var tree = new DialogueTree();
            for (int i = 0; i < lines.Count; i++)
            {
                bool isLast = i == lines.Count - 1;
                var node = new DialogueNode
                {
                    NodeID = $"{entityId}_{i}",
                    Text = lines[i],
                    Choices = isLast
                        ? new List<DialogueChoice>()
                        : new List<DialogueChoice>
                        {
                            new DialogueChoice
                            {
                                Text = "Continue",
                                NextNodeID = $"{entityId}_{i + 1}",
                                Action = null
                            }
                        }
                };
                tree.AddNode(node);
            }
            return tree;
        }
    }
}   