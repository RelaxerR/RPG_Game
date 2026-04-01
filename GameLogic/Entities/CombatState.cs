namespace RPG_Game.GameLogic.Entities;

public class CombatState(int sceneId, string name, int maxHealth, bool isActive = true)
{
    public bool IsActive { get; init; } = isActive;
    
    public string EnemyName { get; set; } = name;
    public int EnemyCurrentHealth { get; set; } = maxHealth;
    public int EnemyMaxHealth { get; set; } = maxHealth;

    public int SceneId { get; set; } = sceneId;

}
