namespace RPG_Game.GameLogic.Entities;

public class Quest
{
    public int Id { get; set; } // ID
    public List<int> FromIds { get; set; } = []; // Откуда можно прийти к этому квесту
    public Dictionary<int, string> ToIds { get; set; } = []; // Куда можно уйти из этого квеста. int - id квеста, string - описание на кнопке (потом будет генерироваться QWEN)
    
    public string Title { get; set; } = string.Empty; // Сообщение при переходе к квесту (Локация: Title)
    public string Description { get; set; } = string.Empty; // Описание (пока будет писаться ровно оно. Потом это будет делать Qwen)
        
    // Балансировочные параметры
    public int DifficultyLevel { get; set; } // Рекомендуемый уровень игрока
    public int MinExperienceReward { get; set; }
    public int MaxExperienceReward { get; set; }
    public int MinCoinsReward { get; set; }
    public int MaxCoinsReward { get; set; }
        
    public bool IsStoryQuest { get; set; } // Ведет ли к прогрессу сюжета или это "гринд"

    // Требования для мирного решения (проверка памяти или навыков)
    public string? RequiredKeyword { get; set; } 
}