namespace RPG_Game.GameLogic.Entities;

public class ActionResultDto 
{
    public Player Player { get; set; } = null!;
    public Quest Quest { get; set; } = null!;
    public List<GameEvent> Events { get; set; } = new();
}