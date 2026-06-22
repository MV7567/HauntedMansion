namespace HauntedMansion.Data
{
    /// <summary>
    /// Loads content to other classes
    /// </summary>
    public interface IContentLoader
    {
        void LoadAll(string path);
        string GetRoomDescription(string roomId);
        //List<string> GetDialogueLines(string entityId);
        //string GetDialogueLine(string lineId);
        
        List<string> GetAllRoomIds();
        JsonContentLoader.EnemyData? GetEnemyData(string enemyId);
        JsonContentLoader.RoomData? GetRoomData(string roomId);
        JsonContentLoader.StatsData GetPlayerDefaultStats();
        List<JsonContentLoader.InteractableData> GetInteractables(string roomId);
        JsonContentLoader.ItemData? GetItemData(string itemId);
        JsonContentLoader.DialogueFileData? GetDialogueData(string entityId);
    }
}