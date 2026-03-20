namespace RPG_Game.API.Models;

public class CombatRequest
{
    public string PlayerName { get; set; } = string.Empty;
    public int CurrentQuestId { get; set; }
    public CombatActionType ActionType { get; set; }
}