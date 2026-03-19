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

    public Quest GetNextQuest(Player player)
    {
        // Логика: если 1 уровень — всегда даем сюжетный старт в таверне
        if (player.Level == 1 && player.Experience == 0)
        {
            return QuestRepository.Quests.First(q => q.IsStoryQuest);
        }
            
        // Иначе — случайный по уровню
        return QuestRepository.GetRandomSideQuest(player.Level);
    }
}