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
        /// optional command when player picks this choice (eg unlock door, get item etc)
        /// null is display only, no game effect
        /// </summary>
        public ICommand Action { get; init; }
    }
}