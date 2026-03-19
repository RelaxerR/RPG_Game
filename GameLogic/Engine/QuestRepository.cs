using RPG_Game.GameLogic.Entities;

namespace RPG_Game.GameLogic.Engine;

public static class QuestRepository
{
    public static List<Quest> Quests =
    [
        new()
        {
            Id = 1,
            Title = "Крысы в подвале",
            Description = "Старая добрая классика. В подвале таверны что-то скребется.",
            DifficultyLevel = 1,
            MinExperienceReward = 20,
            MaxExperienceReward = 40,
            IsStoryQuest = false,
            LocationType = "Tavern"
        },

        new()
        {
            Id = 100, // Сюжетный
            Title = "Таинственный незнакомец",
            Description = "В углу сидит человек в капюшоне. Он шепчет имя: 'Эльдора'.",
            DifficultyLevel = 1,
            MinExperienceReward = 50,
            MaxExperienceReward = 50,
            IsStoryQuest = true,
            LocationType = "Tavern",
            RequiredKeyword = "Эльдора" // Игрок должен будет это запомнить!
        }
    ];

    public static Quest GetRandomSideQuest(int level) 
        => Quests.Where(q => !q.IsStoryQuest && q.DifficultyLevel <= level)
            .OrderBy(x => Guid.NewGuid()).First();
}