namespace RPG_Game.GameLogic.Entities;

public class ActionResultDto 
{
    public Player Player { get; set; } = null!;
    public Scene Scene { get; set; } = null!;
    public List<GameEvent> Events { get; set; } = new();
}