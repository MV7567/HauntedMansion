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
        private DialogueNode _currentNode;
        private DialogueTree _currentTree;
        private IDialoguable _currentEntity;

        public bool IsActive { get; private set; }

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
            _currentEntity = entity;
            _currentTree = LoadTree(entity.GetStartingNode());

            if (_currentTree == null)
            {
                Console.WriteLine("[No dialogue found]");
                return;
            }

            _currentNode = _currentTree.GetNode(entity.GetStartingNode());
            IsActive = true;
            DisplayCurrentNode();
        }

        public void SelectChoice(int choiceIndex, Player player, CombatContext context = null)
        {
            if (!IsActive || _currentNode == null) return;
            if (choiceIndex < 0 || choiceIndex >= _currentNode.Choices.Count)
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            var choice = _currentNode.Choices[choiceIndex];

            // execute game effect (if it exists)
            choice.Action?.Execute(context);

            // move to next node
            if (string.IsNullOrEmpty(choice.NextNodeID))
            {
                EndConversation();
                return;
            }

            _currentNode = _currentTree.GetNode(choice.NextNodeID);

            if (_currentNode == null || _currentNode.IsTerminal)
            {
                if (_currentNode != null)
                    Console.WriteLine(_currentNode.Text);
                EndConversation();
                return;
            }

            DisplayCurrentNode();
        }

        /// <summary>
        /// returns current choices for irenderer to display
        /// </summary>
        /// <returns></returns>
        public List<DialogueChoice> GetCurrentChoices()
        {
            return _currentNode?.Choices ?? new List<DialogueChoice>();
        }

        public void EndConversation()
        {
            IsActive = false;
            _currentNode = null;
            _currentTree = null;
            _currentEntity = null;
            Console.WriteLine("[Conversation ended]");
        }

        // Private helpers
        private void DisplayCurrentNode()
        {
            if (_currentNode == null) return;
            Console.WriteLine($"\n{_currentNode.Text}");

            for (int i = 0; i < _currentNode.Choices.Count; i++)
                Console.WriteLine($" {i + 1}. {_currentNode.Choices[i].Text}");
        }

        /// <summary>
        /// builds dialogue tree form json via Content loader
        /// </summary>
        private DialogueTree LoadTree(string entityId)
        {
            var lines = _loader.GetDialogueLines(entityId);
            if (lines == null || lines.Count == 0) return null;

            // future: parse full choice structure from file
            // for now build simple linear tree from lines list
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