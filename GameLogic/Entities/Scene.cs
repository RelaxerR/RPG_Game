namespace RPG_Game.GameLogic.Entities;

public class Scene
{
    public int Id { get; init; }
    public List<int> FromIds { get; set; } = [];
    public Dictionary<int, string> ToIds { get; set; } = [];
    public string Title { get; set; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    
    // Балансировочные параметры
    public int DifficultyLevel { get; set; }
    public int MinExperienceReward { get; init; }
    public int MaxExperienceReward { get; init; }
    public string ExperienceReason { get; set; } = string.Empty;
    public int MinGoldReward { get; init; }
    public int MaxGoldReward { get; init; }
    public string GoldReason { get; set; } = string.Empty;
    public bool IsStoryQuest { get; set; }
    public string? RequiredKeyword { get; set; }
    
    // === БОЕВАЯ СИСТЕМА ===
    public bool HasCombat { get; init; }
    public string? EnemyName { get; init; }
    public int EnemyMaxHealth { get; init; }
    public int EnemyDamage { get; init; }
    public int EnemyArmor { get; init; }
    public int EnemyExperienceReward { get; init; }
    public int EnemyGoldReward { get; init; }
    public int NegotiationSuccessChance { get; init; }
    public string? NegotiationSuccessText { get; set; }
    public string? NegotiationFailText { get; set; }
    public int CombatWinNextSceneId { get; init; }
    public int CombatFleeNextSceneId { get; init; }
}
