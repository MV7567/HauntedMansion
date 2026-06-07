namespace HauntedMansion.Data
{
    /// <summary>
    /// Loads content to other classes
    /// </summary>
    public interface IContentLoader
    {
        void LoadAll();
        string GetRoomDescription(string roomId);
        List<string> GetDialogueLines(string roomId);
        string GetItemDescription(string itemId);
        string GetDialogueLine(string lineId);
    }
}