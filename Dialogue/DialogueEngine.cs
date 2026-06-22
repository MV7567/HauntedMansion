using HauntedMansion.Combat;
using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Data;
using HauntedMansion.Entities;

namespace HauntedMansion.Dialogue
{
    public class DialogueEngine
    {
        private readonly IContentLoader _loader;
        private DialogueNode? _currentNode;
        private DialogueTree? _currentTree;

        public bool IsActive { get; private set; }
        public DialogueNode? CurrentNode => _currentNode;
        
        public DialogueEngine(IContentLoader loader)
        {
            _loader = loader;
        }
        
        public void StartConversation(IDialoguable entity, Player player)
        {
            // base key and current node
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
            _currentNode?.Choices ?? new List<DialogueChoice>();

        public void EndConversation()
        {
            IsActive = false;
            _currentNode = null;
            _currentTree = null;
        }

        private DialogueTree? LoadTree(string entityId, Enemy? enemyRef, Player player)
        {
            if (string.IsNullOrEmpty(entityId)) return null;
            
            var data = _loader.GetDialogueData(entityId);
            if (data == null) return null;

            var tree = new DialogueTree();

            // 1. advanced tree (for bosses)
            if (data.Nodes != null && data.Nodes.Count > 0)
            {
                foreach (var kvp in data.Nodes)
                {
                    var nodeData = kvp.Value;
                    var node = new DialogueNode { NodeID = kvp.Key, Text = nodeData.Text ?? "" };

                    if (nodeData.Choices != null)
                    {
                        foreach (var cData in nodeData.Choices)
                        {
                            // if the choice requires an item and the player doesnt have it, skip
                            if (!string.IsNullOrEmpty(cData.RequiredItem) && !player.PlayerInventory.HasKeyItem(cData.RequiredItem))
                                continue; 

                            IDialogueAction? action = null;
                            if (!string.IsNullOrEmpty(cData.ActionType) && enemyRef != null)
                            {
                                // states fromjson
                                ICombatState targetState = cData.State switch {
                                    "Sparable" => new HauntedMansion.Combat.States.SparableState(),
                                    "Aggressive" => new HauntedMansion.Combat.States.AggressiveState(),
                                    "Weakened" => new HauntedMansion.Combat.States.WeakenedState(),
                                    _ => new HauntedMansion.Combat.States.AggressiveState()
                                };

                                if (cData.ActionType == "ItemAndState" && !string.IsNullOrEmpty(cData.RequiredItem))
                                    action = new HauntedMansion.Dialogue.Actions.ItemAndStateDialogueAction(enemyRef, targetState, cData.RequiredItem, player, cData.ActionMessage ?? "");
                                else if (cData.ActionType == "SetState")
                                    action = new HauntedMansion.Dialogue.Actions.SetStateDialogueAction(enemyRef, targetState, cData.ActionMessage ?? "");
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

            // 2. simple (for normal enemies)
            if (data.Lines != null && data.Lines.Count > 0)
            {
                for (int i = 0; i < data.Lines.Count; i++)
                {
                    bool isLast = i == data.Lines.Count - 1;
                    
                    // the first node has to be names the same as startingNodeId from enemies.json
                    string nodeId = i == 0 ? entityId : $"{entityId}_{i}"; 
                    
                    var node = new DialogueNode
                    {
                        NodeID = nodeId,
                        Text = data.Lines[i],
                        Choices = isLast ? new List<DialogueChoice>() : new List<DialogueChoice>
                        {
                            new DialogueChoice { Text = "Next...", NextNodeID = $"{entityId}_{i + 1}", Action = null }
                        }
                    };
                    tree.AddNode(node);
                }
                return tree;
            }

            return null;
        }
    }
}