using HauntedMansion.Combat;
using HauntedMansion.Data;
using HauntedMansion.Dialogue.Actions;
using HauntedMansion.Entities;

namespace HauntedMansion.Dialogue
{
    /// <summary>
    /// Controls the flow of conversation
    /// It keeps track of the CurrentNode and moves to the NextNode based on user input.
    /// It also evaluates conditions (like checking the player's inventory) before generating choices.
    /// </summary>
    public class DialogueEngine(IContentLoader loader)
    {
        private DialogueNode? _currentNode;
        private DialogueTree? _currentTree;

        public bool IsActive { get; private set; }
        public DialogueNode? CurrentNode => _currentNode;
        
        // Initializes the conversation graph
        public void StartConversation(IDialoguable entity, Player player)
        {
            // Base key and current node
            string treeId = entity.GetBaseTreeId(); 
            string startNodeId = entity.GetStartingNode(); 

            _currentTree = LoadTree(treeId, entity as Enemy, player);

            if (_currentTree == null)
            {
                IsActive = false;
                return;
            }

            _currentNode = _currentTree.GetNode(startNodeId); 
            IsActive = _currentNode != null;
        }

        // Processes the player's choice, executes any attached Commands, and moves to the next Node
        public string SelectChoice(int choiceIndex, Player player, CombatContext? context = null)
        {
            if (!IsActive || _currentNode == null) return string.Empty;

            if (choiceIndex < 0 || choiceIndex >= _currentNode.Choices.Count)
                return "Invalid choice.";

            var choice = _currentNode.Choices[choiceIndex];
            string actionMessage = string.Empty;

            if (choice.Action != null)
                actionMessage = choice.Action.Execute(context) ?? string.Empty;

            _currentNode = _currentTree?.GetNode(choice.NextNodeID);

            if (_currentNode == null || _currentNode.IsTerminal)
                IsActive = false;

            return actionMessage;
        }

        public List<DialogueChoice> GetCurrentChoices() =>
            _currentNode?.Choices ?? [];

        public void EndConversation()
        {
            IsActive = false;
            _currentNode = null;
            _currentTree = null;
        }

        // Builds the tree from JSON data
        private DialogueTree? LoadTree(string entityId, Enemy? enemyRef, Player player)
        {
            if (string.IsNullOrEmpty(entityId)) return null;
            
            var data = loader.GetDialogueData(entityId);
            if (data == null) return null;

            var tree = new DialogueTree();

            // 1. Advanced tree (for bosses and complex NPCs)
            if (data.Nodes is { Count: > 0 })
            {
                foreach (var kvp in data.Nodes)
                {
                    var nodeData = kvp.Value;
                    var node = new DialogueNode { NodeID = kvp.Key, Text = nodeData.Text ?? "" };

                    if (nodeData.Choices != null)
                    {
                        foreach (var cData in nodeData.Choices)
                        {
                            // If the choice requires an item and the player doesn't have it, skip
                            if (!string.IsNullOrEmpty(cData.RequiredItem) && !player.PlayerInventory.HasKeyItem(cData.RequiredItem))
                                continue; 

                            IDialogueAction? action = null;
                            if (!string.IsNullOrEmpty(cData.ActionType) && enemyRef != null)
                            {
                                // Map strings from JSON to actual Command Objects
                                string targetState = cData.State ?? "";

                                if (cData.ActionType == "ItemAndState" && !string.IsNullOrEmpty(cData.RequiredItem))
                                {
                                    action = new ItemAndStateDialogueAction(enemyRef, targetState, cData.RequiredItem, player, cData.ActionMessage ?? "");
                                }
                                else if (cData.ActionType == "SetState")
                                {
                                    action = new SetStateDialogueAction(enemyRef, targetState, cData.ActionMessage ?? "");
                                }
                            }

                            node.Choices.Add(new DialogueChoice 
                            {
                                Text = cData.Text ?? "",
                                NextNodeID = cData.NextNodeId ?? "",
                                Action = action
                            });
                        }
                    }
                    tree.AddNode(node);
                }
                return tree;
            }

            // 2. Simple linear tree (for normal enemies)
            if (data.Lines is { Count: > 0 })
            {
                for (int i = 0; i < data.Lines.Count; i++)
                {
                    bool isLast = i == data.Lines.Count - 1;
                    
                    // The first node has to be named the same as startingNodeId from enemies.json
                    string nodeId = i == 0 ? entityId : $"{entityId}_{i}"; 
                    
                    var node = new DialogueNode
                    {
                        NodeID = nodeId,
                        Text = data.Lines[i],
                        Choices = isLast ? [] :
                        [
                            new DialogueChoice { Text = "Next...", NextNodeID = $"{entityId}_{i + 1}", Action = null }
                        ]
                    };
                    tree.AddNode(node);
                }
                return tree;
            }

            return null;
        }
    }
}