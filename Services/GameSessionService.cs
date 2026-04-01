using Microsoft.Extensions.Logging;
using RPG_Game.API.Models;
using RPG_Game.GameLogic.Engine;
using RPG_Game.GameLogic.Entities;

namespace RPG_Game.Services;

public class GameSessionService(ILogger<GameSessionService> logger)
{
    private readonly Dictionary<string, Player> _activePlayers = [];
    private readonly Random _rnd = new();

    private Player CreatePlayer(string name)
    {
        logger.LogInformation("Creating new player: {PlayerName}", name);
        var player = new Player { Name = name };
        _activePlayers[name] = player;
        logger.LogDebug("Player {PlayerName} initialized with HP: {Health}, Level: {Level}", 
            name, player.CurrentHealth, player.Level);
        return player;
    }

    public Player GetOrCreatePlayer(string name)
    {
        if (!_activePlayers.TryGetValue(name, out var player))
            return CreatePlayer(name);
        
        logger.LogDebug("Retrieved existing player: {PlayerName}", name);
        return player;
    }
    
    public ActionResultDto MoveToQuest(Player player, int nextQuestId)
    {
        logger.LogDebug("Player {PlayerName} attempting to move to quest {QuestId}", 
            player.Name, nextQuestId);
        
        // Проверка на смерть
        if (!player.IsAlive)
        {
            logger.LogWarning("Player {PlayerName} is dead, redirecting to start scene", player.Name);
            return new ActionResultDto
            {
                Player = player,
                Scene = QuestRepository.Quests.First(q => q.Id == 0),
                Events = [new GameEvent("Вы погибли")]
            };
        }
        
        var events = new List<GameEvent>();
        
        var nextQuest = QuestRepository.Quests.FirstOrDefault(q => q.Id == nextQuestId)
                        ?? QuestRepository.Quests.First(x => x.Id == 666);
        
        logger.LogDebug("Resolved quest {QuestId}: HasCombat={HasCombat}, Title={Title}", 
            nextQuest.Id, nextQuest.HasCombat, nextQuest.Title);

        // === ПРОВЕРКА НА НАЧАЛО НОВОГО БОЯ ===
        if (nextQuest.HasCombat && player.ActiveCombat == null)
        {
            logger.LogInformation("Player {PlayerName} entering combat on quest {QuestId}", 
                player.Name, nextQuest.Id);
            player.StartCombat(nextQuest);
            player.ActiveCombat.NegotiationChance = nextQuest.Combat.NegotiationSuccessChance;
            events.Add(new GameEvent($"Бой начался (Враг: {nextQuest.Combat.EnemyName})"));
    
            logger.LogDebug("Combat state initialized: EnemyHP={EnemyHP}, EnemyDamage={EnemyDamageMin}-{EnemyDamageMin}", 
                player.ActiveCombat?.EnemyCurrentHealth, nextQuest.Combat.EnemyDamageMin, nextQuest.Combat.EnemyDamageMax);
            
            return new ActionResultDto { Player = player, Scene = nextQuest, Events = events };
        }
        
        // === ОБЫЧНЫЙ ПЕРЕХОД (без боя) ===
        logger.LogTrace("Applying rewards for quest {QuestId}", nextQuestId);
        ApplyRewards(player, nextQuest, events);
        
        logger.LogInformation("Player {PlayerName} moved to quest {QuestId} (Gold: {Gold}, XP: {Exp}, Level: {Level})", 
            player.Name, nextQuest.Id, player.Gold, player.Experience, player.Level);
        
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

        logger.LogDebug("Player {PlayerName} combat action: {ActionType} on quest {QuestId}", 
            player.Name, actionType, currentQuestId);

        switch (actionType)
        {
            // === АТАКА ИГРОКА ===
            case CombatActionType.Attack:
            {
                var playerDamage = player.RollDamage();
                var damage = Math.Max(1, playerDamage - scene.Combat.EnemyArmor);
                combat.EnemyCurrentHealth -= damage;
                
                logger.LogTrace("Attack calculation: RawDamage={Raw}, AfterArmor={After}, EnemyHP={HP}", 
                    playerDamage, damage, combat.EnemyCurrentHealth);
                
                events.Add(new GameEvent($"Вы атаковали \"{scene.Combat.EnemyName}\""));
                events.Add(new GameEvent(scene.Combat.GetDamage2EnemyReason(damage, player.GetMaxDamage())));
            
                if (combat.EnemyCurrentHealth <= 0)
                {
                    logger.LogInformation("Player {PlayerName} defeated enemy on quest {QuestId}", 
                        player.Name, currentQuestId);
                    
                    player.EndCombat();
                    events.Add(new GameEvent($"Вы получили опыт за победу над \"{scene.Combat.EnemyName}\"!"));
                    
                    if (player.AddExperience(scene.Combat.EnemyExperienceReward))
                    {
                        events.Add(new GameEvent($"У вас новый уровень ({player.Level})!"));
                        logger.LogInformation("Player {PlayerName} leveled up to {NewLevel}", 
                            player.Name, player.Level);
                    }
                    
                    player.AddGold(scene.Combat.EnemyGoldReward);
                    logger.LogDebug("Rewards granted: Gold={Gold}, XP={XP}", 
                        scene.Combat.EnemyGoldReward, scene.Combat.EnemyExperienceReward);
                
                    var winScene = QuestRepository.Quests.FirstOrDefault(q => q.Id == scene.Combat.CombatWinNextSceneId) ?? scene;
                    return new ActionResultDto { Player = player, Scene = winScene, Events = events };
                }
                break;
            }
            // === ПОБЕГ ===
            case CombatActionType.Flee:
            {
                var fleeDamage = _rnd.Next(5, 15);
                player.TakeDamage(fleeDamage);
                logger.LogTrace("Flee attempt: DamageTaken={Damage}, PlayerHP={HP}", 
                    fleeDamage, player.CurrentHealth);
                
                events.Add(new GameEvent("Вы убегаете"));
            
                if (!player.IsAlive)
                {
                    logger.LogWarning("Player {PlayerName} died while fleeing", player.Name);
                    player.EndCombat();
                    events.Add(new GameEvent("Вы погибли"));
                    var first = QuestRepository.Quests.First(q => q.Id == 0);
                    return new ActionResultDto { Player = player, Scene = first, Events = events };
                }
            
                player.EndCombat();
                var partialExp = scene.Combat.EnemyExperienceReward / 3;
                player.AddExperience(partialExp);
                events.Add(new GameEvent("Вы получили опыт", GameEventType.Exp));
                logger.LogDebug("Flee success: Partial XP granted={XP}", partialExp);
            
                var fleeScene = QuestRepository.Quests.FirstOrDefault(q => q.Id == scene.Combat.CombatFleeNextSceneId) ?? scene;
                return new ActionResultDto { Player = player, Scene = fleeScene, Events = events };
            }
            // === ПЕРЕГОВОРЫ ===
            case CombatActionType.Negotiate:
            {
                var successRoll = _rnd.Next(0, 100);
                var chance = scene.Combat.NegotiationSuccessChance - scene.NegotiateTrys * 10;
                if (chance < 1) chance = 1;
                combat.NegotiationChance = chance;
                
                scene.NegotiateTrys++;
                logger.LogTrace("Negotiation roll: {Roll} vs Chance={Chance}", 
                    successRoll, chance);
                
                if (successRoll < chance)
                {
                    logger.LogInformation("Player {PlayerName} succeeded in negotiation", player.Name);
                    player.EndCombat();
                    events.Add(new GameEvent("Вы получили золото за то что смогли договориться", GameEventType.Gold));
                    var peaceScene = QuestRepository.Quests.FirstOrDefault(q => q.Id == scene.Combat.CombatWinNextSceneId) ?? scene;
                    return new ActionResultDto { Player = player, Scene = peaceScene, Events = events };
                }
                logger.LogDebug("Negotiation failed for player {PlayerName}", player.Name);
                events.Add(new GameEvent("Вам не удалось договориться"));
                break;
            }
            default:
                logger.LogError("Unknown combat action type: {ActionType} for player {PlayerName}", 
                    actionType, player.Name);
                throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
        }
        
        // === ХОД ВРАГА ===
        if (player.ActiveCombat?.IsActive != true || !player.IsAlive)
        {
            logger.LogDebug("Combat ended or player dead, skipping enemy turn");
            return new ActionResultDto { Player = player, Scene = scene, Events = events };
        }
        
        var enemyDamage = scene.Combat.RollEnemyDamage();
        var playerFinalDamage = Math.Max(1, enemyDamage - player.Armor);
        player.TakeDamage(playerFinalDamage);
        
        logger.LogTrace("Enemy attack: BaseDamage={Base}, AfterArmor={Final}, PlayerHP={HP}", 
            enemyDamage, playerFinalDamage, player.CurrentHealth);
        
        events.Add(new GameEvent("Враг атакует"));
        events.Add(new GameEvent(scene.Combat.GetDamage2PlayerReason(enemyDamage, scene.Combat.EnemyDamageMax)));

        if (player.IsAlive)
        {
            logger.LogDebug("Combat continues: PlayerHP={HP}, EnemyHP={EnemyHP}", 
                player.CurrentHealth, combat.EnemyCurrentHealth);
            return new ActionResultDto { Player = player, Scene = scene, Events = events };
        }
        
        logger.LogWarning("Player {PlayerName} died in combat on quest {QuestId}", player.Name, currentQuestId);
        player.EndCombat();
        events.Add(new GameEvent("Вы погибли"));
        var startScene = QuestRepository.Quests.First(q => q.Id == 0);
        return new ActionResultDto { Player = player, Scene = startScene, Events = events };
    }
    
    private void ApplyRewards(Player player, Scene quest, List<GameEvent> events)
    {
        var gold = quest.GoldReward.GetReward();
        if (gold > 0)
        {
            player.Gold += gold;
            events.Add(new GameEvent("Вы получили золото", GameEventType.Gold));
            logger.LogTrace("Gold reward: {Gold}", gold);
        }
        
        var oldLevel = player.Level;
        var exp = quest.ExpReward.GetReward();
        if (exp <= 0)
        {
            logger.LogTrace("No experience reward for quest {QuestId}", quest.Id);
            return;
        }
        
        player.AddExperience(exp);
        events.Add(new GameEvent("Вы получили опыт", GameEventType.Exp));
        logger.LogTrace("Experience reward: {Exp}", exp);

        if (player.Level <= oldLevel)
            return;
        
        events.Add(new GameEvent("У вас новый уровень!", GameEventType.LevelUp));
        logger.LogInformation("Player {PlayerName} leveled up from {OldLevel} to {NewLevel}", 
            player.Name, oldLevel, player.Level);
    }
}