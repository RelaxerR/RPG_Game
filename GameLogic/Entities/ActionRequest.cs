namespace RPG_Game.GameLogic.Entities;

// Этот класс описывает структуру JSON, который придет с фронтенда
public class ActionRequest
{
    public string PlayerName { get; set; } = string.Empty;
    public int TargetQuestId { get; set; }
}