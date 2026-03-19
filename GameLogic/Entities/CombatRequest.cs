namespace RPG_Game.GameLogic.Entities;

public class CombatRequest
{
    public string PlayerName { get; set; } = string.Empty;
    public int CurrentQuestId { get; set; }
    public int ActionType { get; set; } // -1 Attack, -2 Flee, -3 Negotiate
}