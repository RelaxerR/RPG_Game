namespace RPG_Game.GameLogic.Entities;

public class Player
{
    public string Name { get; set; } = "Странник";
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public int Gold { get; set; } = 0;
        
    // Характеристики для баланса
    public int MaxHealth { get; set; } = 100;
    public int CurrentHealth { get; set; } = 100;
    public int Armor { get; set; } = 0; // Плоское снижение урона
    public int BaseDamage { get; set; } = 10;

    // Инвентарь (простая реализация для прототипа)
    public List<string> Inventory { get; set; } = new();
    public List<string> KnownKeywords { get; set; } = new(); // Память игрока (сюжетные зацепки)

    // --- Математика прогрессии ---

    // Формула необходимого опыта для уровня: Level * 100 (простая линейная для 10-минутной игры)
    public int ExperienceToNextLevel
    {
        get => Level ^ 2;
    }

    public void AddExperience(int amount)
    {
        Experience += amount;
        if (Experience >= ExperienceToNextLevel)
        {
            LevelUp();
        }
    }
    
    public void AddGold(int amount)
    {
        Gold += amount;
    }

    private void LevelUp()
    {
        Level++;
        Experience = 0; // Сбрасываем или вычитаем остаток
        MaxHealth += 20;
        CurrentHealth = MaxHealth; // Лечим при повышении уровня
        BaseDamage += 5;
        // Здесь можно добавить логику уведомления игрока
    }

    // Расчет наносимого урона с небольшим разбросом (Random)
    public int RollDamage()
    {
        var rnd = new Random();
        return rnd.Next(BaseDamage, BaseDamage + (Level * 2));
    }

    // Получение урона с учетом брони
    public void TakeDamage(int rawDamage)
    {
        var finalDamage = Math.Max(1, rawDamage - Armor); // Минимум 1 урон всегда
        CurrentHealth -= finalDamage;
        if (CurrentHealth < 0) CurrentHealth = 0;
    }

    public bool IsAlive
    {
        get => CurrentHealth > 0;
    }
}