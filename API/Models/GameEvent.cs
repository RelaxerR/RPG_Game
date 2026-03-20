namespace RPG_Game.API.Models;

public class GameEvent
{
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Info"; // Exp, Gold, LevelUp, Info
}