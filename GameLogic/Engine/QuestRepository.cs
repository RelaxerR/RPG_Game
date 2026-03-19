using RPG_Game.GameLogic.Entities;

namespace RPG_Game.GameLogic.Engine;

public static class QuestRepository
{
    public static List<Scene> Quests =
    [
        new()
        {
            Id = 0,
            Title = "Таверна 'Пьяный Дракон'",
            Description = "Свет свечей дрожит, пахнет элем и жареным мясом. Старый трактирщик косится на вас.",
            ToIds = new Dictionary<int, string> { { 1, "Спуститься в подвал" }, { 2, "Выйти на тракт" } },
            IsStoryQuest = true,
            DifficultyLevel = 1
        },
        new()
        {
            Id = 1,
            FromIds = [0],
            Title = "Темный подвал",
            Description = "Здесь пахнет сыростью. Среди бочек слышен писк огромных крыс.",
            ToIds = new Dictionary<int, string> { { 0, "Вернуться в зал" } },
            DifficultyLevel = 1,
        },
        new()
        {
            Id = 2,
            FromIds = [0],
            Title = "Лесная дорога",
            Description = "Путь преграждает поваленное дерево. Из-за него выходит вооружённый разбойник с ножом.",
            ToIds = new Dictionary<int, string>
            {
                { 3, "Искать обход" },
                { 4, "Идти напролом" },
                { 5, "Попытаться договориться" }
            },
            IsStoryQuest = true,
            MinExperienceReward = 1,
            MaxExperienceReward = 5,
            ExperienceReason = "Выход из таверны",
            MinGoldReward = 5,
            MaxGoldReward = 10,
            GoldReason = "Путешествие",
            DifficultyLevel = 1,
            
            // === ПАРАМЕТРЫ БОЯ ===
            HasCombat = true,
            EnemyName = "Лесной разбойник",
            EnemyMaxHealth = 50,
            EnemyDamage = 8,
            EnemyArmor = 2,
            EnemyExperienceReward = 25,
            EnemyGoldReward = 15,
            NegotiationSuccessChance = 40,
            NegotiationSuccessText = "Разбойник опускает нож: 'Ладно, проходи.'",
            NegotiationFailText = "Разбойник смеётся: 'На смерть!' и бросается в атаку.",
            CombatWinNextSceneId = 3,
            CombatFleeNextSceneId = 3
        },
        new()
        {
            Id = 3,
            Title = "Перекрёсток",
            Description = "Дорога расходится в три стороны. Куда пойдёте?",
            ToIds = new Dictionary<int, string> { { 0, "Вернуться в таверну" } },
            DifficultyLevel = 1
        },
        new()
        {
            Id = 666,
            Title = "Лимбо",
            Description = "Вы попали в Лимбо. Вернитесь в таверну.",
            ToIds = new Dictionary<int, string> { { 0, "В таверну!" } },
            IsStoryQuest = false,
            DifficultyLevel = 0
        }
    ];
}