namespace RPG_Game.GameLogic.Entities;

public class Scene
{
    public int Id { get; set; }
    public List<int> FromIds { get; set; } = [];
    public Dictionary<int, string> ToIds { get; set; } = [];
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Балансировочные параметры
    public int DifficultyLevel { get; set; }
    public int MinExperienceReward { get; set; }
    public int MaxExperienceReward { get; set; }
    public string ExperienceReason { get; set; } = string.Empty;
    public int MinGoldReward { get; set; }
    public int MaxGoldReward { get; set; }
    public string GoldReason { get; set; } = string.Empty;
    public bool IsStoryQuest { get; set; }
    public string? RequiredKeyword { get; set; }
    
    // === БОЕВАЯ СИСТЕМА ===
    public bool HasCombat { get; set; } = false;
    public string? EnemyName { get; set; }
    public int EnemyMaxHealth { get; set; }
    public int EnemyDamage { get; set; }
    public int EnemyArmor { get; set; }
    public int EnemyExperienceReward { get; set; }
    public int EnemyGoldReward { get; set; }
    public int NegotiationSuccessChance { get; set; } = 0;
    public string? NegotiationSuccessText { get; set; }
    public string? NegotiationFailText { get; set; }
    public int CombatWinNextSceneId { get; set; }
    public int CombatFleeNextSceneId { get; set; }
}
