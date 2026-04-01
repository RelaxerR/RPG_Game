namespace RPG_Game.GameLogic.Entities;

public class Scene
{
    private static readonly Random _rnd = new Random();
    
    public required int Id { get; init; }
    public Dictionary<int, string> ToIds { get; set; } = [];
    public required string Title { get; set; }
    public required string Description { get; init; }
    
    // Балансировочные параметры
    public required int DifficultyLevel { get; set; }
    public required RandomReward ExpReward { get; set; }
    public required RandomReward GoldReward { get; set; }
    public required bool IsStoryQuest { get; set; }
    
    // === БОЕВАЯ СИСТЕМА ===
    public bool HasCombat { get; init; }
    public int NegotiateTrys { get; set; } = 0;
    public required CombatScenePart Combat { get; init; }
}
