using RPG_Game.GameLogic.Entities;

namespace RPG_Game.GameLogic.Engine;

public static class QuestRepository
{
    public static List<Scene> Quests =
    [
        new Scene
        {
            Id = 0, // Стартовая точка
            Title = "Таверна 'Пьяный Дракон'",
            Description = "Свет свечей дрожит, пахнет элем и жареным мясом. Старый трактирщик косится на вас.",
            ToIds = new Dictionary<int, string> { { 1, "Спуститься в подвал" }, { 2, "Выйти на тракт" } },
            IsStoryQuest = true,
            DifficultyLevel = 1
        },

        new Scene
        {
            Id = 1,
            FromIds = [0],
            Title = "Темный подвал",
            Description = "Здесь пахнет сыростью. Среди бочек слышен писк огромных крыс.",
            ToIds = new Dictionary<int, string> { { 0, "Вернуться в зал" } }, // Можно вернуться назад
            DifficultyLevel = 1,
        },

        new Scene
        {
            Id = 2,
            FromIds = [0],
            Title = "Лесная дорога",
            Description = "Путь преграждает поваленное дерево. Похоже на засаду.",
            ToIds = new Dictionary<int, string> { { 3, "Искать обход" }, { 4, "Идти напролом" } },
            IsStoryQuest = true,
            MinExperienceReward = 1,
            MaxExperienceReward = 5,
            ExperienceReason = "Выход из таверны",
            MinGoldReward = 5,
            MaxGoldReward = 10,
            GoldReason = "Путешествие",
            DifficultyLevel = 1
        },
        
        new Scene
        {
            Id = 666,
            Title = "Лимбо",
            Description = "Вы попали в Лимбо. Это могло произойти из-за ошибки игры. Вы можете вернуться на стартовую локацию. Будем рады, если сообщите нам, что вы делали, чтобы попасть сюда. Спасибо за помощь в  развитии игры!",
            ToIds = new Dictionary<int, string> { { 0, "В таверну!" } },
            IsStoryQuest = false,
            DifficultyLevel = 0
        }
    ];
}
