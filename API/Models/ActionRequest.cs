namespace RPG_Game.API.Models;

// Этот класс описывает структуру JSON, который придет с фронтенда
public class ActionRequest(string playerName, int currentQuestId, int targetQuestId)
{
    public string PlayerName { get; } = playerName;
    public int CurrentQuestId { get; } = currentQuestId;
    public int TargetQuestId { get; } = targetQuestId;
}
