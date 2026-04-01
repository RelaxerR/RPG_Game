namespace RPG_Game.GameLogic.Entities;

public struct CombatScenePart
{
    private static readonly Random _rnd = new Random();
    
    public required string EnemyName { get; init; }
    public required int EnemyMaxHealth { get; init; }
    public required int EnemyDamageMin { get; init; }
    public required int EnemyDamageMax { get; init; }
    public int RollEnemyDamage() => _rnd.Next(EnemyDamageMin, EnemyDamageMax);
    public required int EnemyArmor { get; init; }
    public required int EnemyExperienceReward { get; init; }
    public required int EnemyGoldReward { get; init; }
    public required int NegotiationSuccessChance { get; init; }
    public required string NegotiationSuccessText { get; set; }
    public required string NegotiationFailText { get; set; }
    public required int CombatWinNextSceneId { get; init; }
    public required int CombatFleeNextSceneId { get; init; }
    public required Dictionary<int, string> Damage2EnemyReasons { get; init; }
    public required Dictionary<int, string> Damage2PlayerReasons { get; init; }

    public string GetDamage2EnemyReason(int damage, int maxDamage)
    {
        var percent = (int)((double)damage / maxDamage * 100);
    
        // Ищем подходящее описание: берем записи, где ключ <= процента, 
        // сортируем по убыванию и берем первое (самое близкое)
        var reason = Damage2EnemyReasons
            .Where(x => x.Key <= percent)
            .OrderByDescending(x => x.Key)
            .FirstOrDefault();

        return reason.Value ?? "Вы промахнулись или нанесли ничтожный урон";
    }

    public string GetDamage2PlayerReason(int damage, int maxDamage)
    {
        var percent = (int)((double)damage / maxDamage * 100);

        var reason = Damage2PlayerReasons
            .Where(x => x.Key <= percent)
            .OrderByDescending(x => x.Key)
            .FirstOrDefault();

        return reason.Value ?? "Враг промахнулся";
    }
}
