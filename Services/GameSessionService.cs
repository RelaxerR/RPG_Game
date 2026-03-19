using RPG_Game.GameLogic.Engine;
using RPG_Game.GameLogic.Entities;

namespace RPG_Game.Services;

public class GameSessionService
{
    private readonly Dictionary<string, Player> _activePlayers = new();
    private readonly Random _rnd = new();
    
    public Player CreatePlayer(string name)
    {
        var player = new Player { Name = name };
        _activePlayers[name] = player;
        return player;
    }
    
    public Player? GetPlayer(string name) => _activePlayers.GetValueOrDefault(name);
    
    public ActionResultDto MoveToQuest(string playerName, int currentQuestId, int nextQuestId)
    {
        var player = GetPlayer(playerName) ?? throw new Exception("Player not found");
        
        // Проверка на смерть
        if (!player.IsAlive)
        {
            return new ActionResultDto
            {
                Player = player,
                Scene = QuestRepository.Quests.First(q => q.Id == 0),
                Events = new List<GameEvent>
                {
                    new() { Message = "ВЫ ПОГИБЛИ. Возвращение в таверну...", Type = "Info" }
                }
            };
        }
        
        var currentQuest = QuestRepository.Quests.FirstOrDefault(q => q.Id == currentQuestId)
            ?? throw new Exception("Current quest not found");
        
        var events = new List<GameEvent>();
        
        // === ЕСЛИ ИДЁТ БОЙ ===
        if (player.ActiveCombat?.IsActive == true)
        {
            return HandleCombatAction(player, currentQuest, nextQuestId, events);
        }
        
        // === ПРОВЕРКА НА НАЧАЛО НОВОГО БОЯ ===
        var nextQuest = QuestRepository.Quests.FirstOrDefault(q => q.Id == nextQuestId)
                        ?? QuestRepository.Quests.First(x => x.Id == 666);

        if (nextQuest.HasCombat && player.ActiveCombat == null)
        {
            player.StartCombat(nextQuest);
            events.Add(new GameEvent
            {
                Message = $"⚔️ БОЙ НАЧАЛСЯ! {nextQuest.EnemyName} (HP: {nextQuest.EnemyMaxHealth})",
                Type = "Info"
            });
    
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
    
    private ActionResultDto HandleCombatAction(Player player, Scene currentQuest, int actionType, List<GameEvent> events)
    {
        var combat = player.ActiveCombat!;
        var scene = currentQuest;
        
        // === АТАКА ИГРОКА ===
        if (actionType == -1)
        {
            var playerDamage = player.RollDamage();
            var enemyDamage = Math.Max(1, playerDamage - scene.EnemyArmor);
            combat.EnemyCurrentHealth -= enemyDamage;
            events.Add(new GameEvent { Message = $"Вы атаковали! -{enemyDamage} HP врагу", Type = "Info" });
            
            if (combat.EnemyCurrentHealth <= 0)
            {
                player.EndCombat();
                player.AddExperience(scene.EnemyExperienceReward);
                player.AddGold(scene.EnemyGoldReward);
                events.Add(new GameEvent { Message = $"🏆 ПОБЕДА! +{scene.EnemyExperienceReward} XP, +{scene.EnemyGoldReward} Gold", Type = "LevelUp" });
                
                var winScene = QuestRepository.Quests.FirstOrDefault(q => q.Id == scene.CombatWinNextSceneId) ?? currentQuest;
                return new ActionResultDto { Player = player, Scene = winScene, Events = events };
            }
        }
        // === ПОБЕГ ===
        else if (actionType == -2)
        {
            var fleeDamage = _rnd.Next(5, 15);
            player.TakeDamage(fleeDamage);
            events.Add(new GameEvent { Message = $"🏃 ПОБЕГ! -{fleeDamage} HP", Type = "Info" });
            
            if (!player.IsAlive)
            {
                player.EndCombat();
                events.Add(new GameEvent { Message = "ВЫ ПОГИБЛИ при попытке побега...", Type = "Info" });
                var startScene = QuestRepository.Quests.First(q => q.Id == 0);
                return new ActionResultDto { Player = player, Scene = startScene, Events = events };
            }
            
            player.EndCombat();
            var partialExp = scene.EnemyExperienceReward / 3;
            player.AddExperience(partialExp);
            events.Add(new GameEvent { Message = $"Вы сбежали! +{partialExp} XP", Type = "Exp" });
            
            var fleeScene = QuestRepository.Quests.FirstOrDefault(q => q.Id == scene.CombatFleeNextSceneId) ?? currentQuest;
            return new ActionResultDto { Player = player, Scene = fleeScene, Events = events };
        }
        // === ПЕРЕГОВОРЫ ===
        else if (actionType == -3)
        {
            var successRoll = _rnd.Next(0, 100);
            if (successRoll < scene.NegotiationSuccessChance)
            {
                player.EndCombat();
                events.Add(new GameEvent { Message = $"✅ {scene.NegotiationSuccessText}", Type = "Gold" });
                var peaceScene = QuestRepository.Quests.FirstOrDefault(q => q.Id == scene.CombatWinNextSceneId) ?? currentQuest;
                return new ActionResultDto { Player = player, Scene = peaceScene, Events = events };
            }
            else
            {
                events.Add(new GameEvent { Message = $"❌ {scene.NegotiationFailText}", Type = "Info" });
            }
        }
        
        // === ХОД ВРАГА ===
        if (player.ActiveCombat?.IsActive == true && player.IsAlive)
        {
            var enemyDamage = scene.EnemyDamage;
            var playerFinalDamage = Math.Max(1, enemyDamage - player.Armor);
            player.TakeDamage(playerFinalDamage);
            events.Add(new GameEvent { Message = $"⚔️ Враг атакует! -{playerFinalDamage} HP", Type = "Info" });
            
            if (!player.IsAlive)
            {
                player.EndCombat();
                events.Add(new GameEvent { Message = "💀 ВЫ ПОГИБЛИ в бою...", Type = "Info" });
                var startScene = QuestRepository.Quests.First(q => q.Id == 0);
                return new ActionResultDto { Player = player, Scene = startScene, Events = events };
            }
        }
        
        return new ActionResultDto { Player = player, Scene = currentQuest, Events = events };
    }
    
    private void ApplyRewards(Player player, Scene quest, List<GameEvent> events)
    {
        var gold = _rnd.Next(quest.MinGoldReward, quest.MaxGoldReward + 1);
        if (gold > 0)
        {
            player.Gold += gold;
            events.Add(new GameEvent { Message = $"+{gold} золотых ({quest.GoldReason})", Type = "Gold" });
        }
        
        var oldLevel = player.Level;
        var exp = _rnd.Next(quest.MinExperienceReward, quest.MaxExperienceReward + 1);
        if (exp > 0)
        {
            player.AddExperience(exp);
            events.Add(new GameEvent { Message = $"+{exp} XP ({quest.ExperienceReason})", Type = "Exp" });
            
            if (player.Level > oldLevel)
            {
                events.Add(new GameEvent { Message = $"УРОВЕНЬ ПОВЫШЕН! Теперь вы {player.Level} уровня.", Type = "LevelUp" });
            }
        }
    }
}