namespace RPG_Game.API.Models;

public class CombatRequest(string playerName, int currentQuestId, CombatActionType actionType)
{
    public string PlayerName { get; } = playerName;
    public int CurrentQuestId { get; } = currentQuestId;
    public CombatActionType ActionType { get; } = actionType;
}