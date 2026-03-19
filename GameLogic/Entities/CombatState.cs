namespace RPG_Game.GameLogic.Entities;

public class CombatState
{
    public bool IsActive { get; set; } = false;
    public int EnemyCurrentHealth { get; set; }
    public int EnemyMaxHealth { get; set; }
    public string EnemyName { get; set; } = string.Empty;
    public int SceneId { get; set; }
    
    // История боя для лога
    public List<string> CombatLog { get; set; } = new();
    
    public void AddLog(string message)
    {
        CombatLog.Add(message);
    }
}
