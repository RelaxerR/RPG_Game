using RPG_Game.API.Models;
using RPG_Game.GameLogic.Engine;
using RPG_Game.GameLogic.Entities;

namespace RPG_Game.Services;

public class GameSessionService
{
    private readonly Dictionary<string, Player> _activePlayers = [];
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    private readonly Random _rnd = new();

    private Player CreatePlayer(string name)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var player = new Player { Name = name };
        _activePlayers[name] = player;
        return player;
    }

    public Player GetOrCreatePlayer(string name)
    {
        return _activePlayers.TryGetValue(name, out var player) ? player : CreatePlayer(name);
    }
    
    public ActionResultDto MoveToQuest(Player player, int nextQuestId)
    {
        // Проверка на смерть
        if (!player.IsAlive)
        {
            return new ActionResultDto
            {
                Player = player,
                Scene = QuestRepository.Quests.First(q => q.Id == 0),
                Events = [new GameEvent("Вы погибли")]
            };
        }
        
        var events = new List<GameEvent>();
        
        // === ПРОВЕРКА НА НАЧАЛО НОВОГО БОЯ ===
        var nextQuest = QuestRepository.Quests.FirstOrDefault(q => q.Id == nextQuestId)
                        ?? QuestRepository.Quests.First(x => x.Id == 666);

        if (nextQuest.HasCombat && player.ActiveCombat == null)
        {
            player.StartCombat(nextQuest);
            events.Add(new GameEvent("Бой начался"));
    
            // ← УБРАТЬ отсюда ход врага! Просто возвращаем состояние боя
            return new ActionResultDto { Player = player, Scene = nextQuest, Events = events };
        }
        
        // === ОБЫЧНЫЙ ПЕРЕХОД (без боя) ===
        ApplyRewards(player, nextQuest, events);
        
        return new ActionResultDto
        {
            Player = player,
            Scene = nextQuest,
            Events = events
        };
    }
    
    public ActionResultDto HandleCombatAction(Player player, int currentQuestId, CombatActionType actionType, List<GameEvent> events)
    {
        var combat = player.ActiveCombat!;
        var scene = QuestRepository.Quests.FirstOrDefault(q => q.Id == currentQuestId)
                           ?? throw new Exception("Current quest not found");

        switch (actionType)
        {
            // === АТАКА ИГРОКА ===
            case CombatActionType.Attack:
            {
                var playerDamage = player.RollDamage();
                var enemyDamage = Math.Max(1, playerDamage - scene.EnemyArmor);
                combat.EnemyCurrentHealth -= enemyDamage;
                events.Add(new GameEvent("Вы атаковали"));
            
                if (combat.EnemyCurrentHealth <= 0)
                {
                    player.EndCombat();
                    events.Add(new GameEvent("Вы получили опыт!"));
                    
                    if (player.AddExperience(scene.EnemyExperienceReward))
                        events.Add(new GameEvent("У вас новый уровень!"));
                    
                    player.AddGold(scene.EnemyGoldReward);
                
                    var winScene = QuestRepository.Quests.FirstOrDefault(q => q.Id == scene.CombatWinNextSceneId) ?? scene;
                    return new ActionResultDto { Player = player, Scene = winScene, Events = events };
                }
                break;
            }
            // === ПОБЕГ ===
            case CombatActionType.Flee:
            {
                var fleeDamage = _rnd.Next(5, 15);
                player.TakeDamage(fleeDamage);
                events.Add(new GameEvent("Вы убегаете"));
            
                if (!player.IsAlive)
                {
                    player.EndCombat();
                    events.Add(new GameEvent("Вы погибли"));
                    var startScene = QuestRepository.Quests.First(q => q.Id == 0);
                    return new ActionResultDto { Player = player, Scene = startScene, Events = events };
                }
            
                player.EndCombat();
                var partialExp = scene.EnemyExperienceReward / 3;
                player.AddExperience(partialExp);
                events.Add(new GameEvent("Вы получили опыт", GameEventType.Exp));
            
                var fleeScene = QuestRepository.Quests.FirstOrDefault(q => q.Id == scene.CombatFleeNextSceneId) ?? scene;
                return new ActionResultDto { Player = player, Scene = fleeScene, Events = events };
            }
            // === ПЕРЕГОВОРЫ ===
            case CombatActionType.Negotiate:
            {
                var successRoll = _rnd.Next(0, 100);
                if (successRoll < scene.NegotiationSuccessChance)
                {
                    player.EndCombat();
                    events.Add(new GameEvent("Вы получили золото за то что смогли договориться", GameEventType.Gold));
                    var peaceScene = QuestRepository.Quests.FirstOrDefault(q => q.Id == scene.CombatWinNextSceneId) ?? scene;
                    return new ActionResultDto { Player = player, Scene = peaceScene, Events = events };
                }
                events.Add(new GameEvent("Вам не удалось договориться"));
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
        }
        
        // === ХОД ВРАГА ===
        if (player.ActiveCombat?.IsActive != true || !player.IsAlive)
            return new ActionResultDto { Player = player, Scene = scene, Events = events };
        {
            var enemyDamage = scene.EnemyDamage;
            var playerFinalDamage = Math.Max(1, enemyDamage - player.Armor);
            player.TakeDamage(playerFinalDamage);
            events.Add(new GameEvent("Враг атакует"));

            if (player.IsAlive)
                return new ActionResultDto { Player = player, Scene = scene, Events = events };
            
            player.EndCombat();
            events.Add(new GameEvent("Вы погибли"));
            var startScene = QuestRepository.Quests.First(q => q.Id == 0);
            return new ActionResultDto { Player = player, Scene = startScene, Events = events };
        }
    }
    
    private void ApplyRewards(Player player, Scene quest, List<GameEvent> events)
    {
        var gold = _rnd.Next(quest.MinGoldReward, quest.MaxGoldReward + 1);
        if (gold > 0)
        {
            player.Gold += gold;
            events.Add(new GameEvent("Вы получили золото", GameEventType.Gold));
        }
        
        var oldLevel = player.Level;
        var exp = _rnd.Next(quest.MinExperienceReward, quest.MaxExperienceReward + 1);
        if (exp <= 0)
            return;
        
        player.AddExperience(exp);
        events.Add(new GameEvent("Вы получили опыт", GameEventType.Exp));
            
        if (player.Level > oldLevel)
        {
            events.Add(new GameEvent("У вас новый уровень!", GameEventType.LevelUp));
        }
    }
}