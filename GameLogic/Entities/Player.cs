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
    public int Armor { get; set; } = 0;
    public int BaseDamage { get; set; } = 10;
    
    // Инвентарь
    public List<string> Inventory { get; set; } = new();
    public List<string> KnownKeywords { get; set; } = new();
    
    // === БОЕВАЯ СИСТЕМА ===
    public CombatState? ActiveCombat { get; set; } = null;
    
    public void StartCombat(Scene scene)
    {
        // TODO: Вынести в настройки имя врага по умолчанию
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        ActiveCombat = new CombatState(scene.Id, scene.EnemyName ?? "Враг", scene.EnemyMaxHealth);
    }
    
    public void EndCombat()
    {
        ActiveCombat = null;
    }
    
    // --- Математика прогрессии ---
    public int ExperienceToNextLevel => Level * 100;
    
    public bool AddExperience(int amount)
    {
        Experience += amount;
        if (Experience < ExperienceToNextLevel)
            return false;
        
        LevelUp();
        return true;
    }
    
    public void AddGold(int amount)
    {
        Gold += amount;
    }
    
    private void LevelUp()
    {
        Level++;
        Experience = 0;
        MaxHealth += 20;
        CurrentHealth = MaxHealth;
        BaseDamage += 5;
    }
    
    public int RollDamage()
    {
        var rnd = new Random();
        return rnd.Next(BaseDamage, BaseDamage + (Level * 2));
    }
    
    public void TakeDamage(int rawDamage)
    {
        var finalDamage = Math.Max(1, rawDamage - Armor);
        CurrentHealth -= finalDamage;
        if (CurrentHealth < 0) CurrentHealth = 0;
    }
    
    public bool IsAlive => CurrentHealth > 0;
}