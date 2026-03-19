namespace RPG_Game.GameLogic.Entities;

public class Quest
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; // Базовое описание для Qwen
        
    // Балансировочные параметры
    public int DifficultyLevel { get; set; } // Рекомендуемый уровень игрока
    public int MinExperienceReward { get; set; }
    public int MaxExperienceReward { get; set; }
        
    public bool IsStoryQuest { get; set; } // Ведет ли к прогрессу сюжета или это "гринд"
    public string LocationType { get; set; } = "Forest"; // Таверна, Лес, Пещера и т.д.

    // Требования для мирного решения (проверка памяти или навыков)
    public string? RequiredKeyword { get; set; } 
}