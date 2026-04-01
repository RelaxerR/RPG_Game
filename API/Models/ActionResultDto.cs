using RPG_Game.GameLogic.Entities;

namespace RPG_Game.API.Models;

public class ActionResultDto 
{
    public Player Player { get; init; } = null!;
    public Scene Scene { get; init; } = null!;
    public List<GameEvent> Events { get; init; } = [];
}