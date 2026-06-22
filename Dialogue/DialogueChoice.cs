using HauntedMansion.UI;

namespace HauntedMansion.Dialogue
{
    /// <summary>
    /// dialogue option the player can select
    /// </summary>
    public class DialogueChoice
    {
        public string Text { get; init; }
        public string NextNodeID { get; init; }
        
        /// <summary>
        /// Optional action executed when this choice is selected.
        /// e.g. SetStateDialogueAction, GrantItemDialogueAction.
        /// Null means no game effect.
        /// </summary>
        public IDialogueAction? Action { get; init; }
    }
}