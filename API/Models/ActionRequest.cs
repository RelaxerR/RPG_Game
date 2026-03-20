namespace RPG_Game.API.Models;

// Этот класс описывает структуру JSON, который придет с фронтенда
public class ActionRequest
{
    public string PlayerName { get; set; } = string.Empty;
    public int CurrentQuestId { get; set; }
    public int TargetQuestId { get; set; }
}