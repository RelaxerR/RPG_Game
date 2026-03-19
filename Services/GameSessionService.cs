using RPG_Game.GameLogic.Engine;
using RPG_Game.GameLogic.Entities;

namespace RPG_Game.Services;

public class GameSessionService
{
    // Храним игроков по имени (в идеале по ID сессии)
    private readonly Dictionary<string, Player> _activePlayers = new();

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
        
        var currentQuest = QuestRepository.Quests.FirstOrDefault(q => q.Id == currentQuestId) 
                           ?? throw new Exception("Current quest not found");
    
        // Ищем следующий квест
        var nextQuest = QuestRepository.Quests.FirstOrDefault(q => q.Id == nextQuestId) 
                        ?? QuestRepository.Quests.First(x => x.Id == 666);
    
        var events = new List<GameEvent>();
        var rnd = new Random();
        
        // 1. Начисляем золото
        var gold = rnd.Next(currentQuest.MinGoldReward, currentQuest.MaxGoldReward + 1);
        if (gold > 0) {
            player.Gold += gold;
            events.Add(new GameEvent { 
                Message = $"+{gold} золотых ({currentQuest.GoldReason})", 
                Type = "Gold" 
            });
        }
    
        // 2. Начисляем опыт и проверяем Level Up
        var oldLevel = player.Level;
        var exp = rnd.Next(currentQuest.MinExperienceReward, currentQuest.MaxExperienceReward + 1);

        if (exp <= 0)
            return new ActionResultDto
            {
                Player = player,
                Quest = nextQuest,
                Events = events
            };
        player.AddExperience(exp);
        events.Add(new GameEvent { 
            Message = $"+{exp} XP ({currentQuest.ExperienceReason})", 
            Type = "Exp" 
        });
        
        if (player.Level > oldLevel) {
            events.Add(new GameEvent { 
                Message = $"УРОВЕНЬ ПОВЫШЕН! Теперь вы {player.Level} уровня.", 
                Type = "LevelUp" 
            });
        }

        return new ActionResultDto { 
            Player = player, 
            Quest = nextQuest, 
            Events = events 
        };
    }
}