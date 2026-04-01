using RPG_Game.GameLogic.Entities;
// ReSharper disable HeapView.ObjectAllocation.Evident

namespace RPG_Game.GameLogic.Engine;

public static class QuestRepository
{
    public static readonly List<Scene> Quests =
    [
        new()
        {
            Id = 0,
            Title = "Таверна 'Пьяный Дракон'",
            Description = "Свет свечей дрожит, пахнет элем и жареным мясом. Старый трактирщик косится на вас.",
            ToIds = new Dictionary<int, string>
            {
                {
                    1, "Спуститься в подвал"
                },
                {
                    2, "Выйти на тракт"
                }
            },
            IsStoryQuest = true,
            DifficultyLevel = 1,
            ExpReward = default,
            GoldReward = default,
            Combat = default
        },
        new()
        {
            Id = 1,
            Title = "Темный подвал",
            Description = "Здесь пахнет сыростью. Среди бочек слышен писк огромных крыс.",
            ToIds = new Dictionary<int, string>
            {
                {
                    0, "Вернуться в зал"
                }
            },
            DifficultyLevel = 1,
            ExpReward = default,
            GoldReward = default,
            IsStoryQuest = false,
            Combat = default,
        },
        new()
        {
            Id = 2,
            Title = "Лесная дорога",
            Description = "Путь преграждает поваленное дерево. Из-за него выходит вооружённый разбойник с ножом.",
            ToIds = new Dictionary<int, string>
            {
                { 3, "Искать обход" },
                { 4, "Идти напролом" },
                { 5, "Попытаться договориться" }
            },
            IsStoryQuest = true,
            ExpReward = new RandomReward(1, 5, "Выход из таверны"),
            GoldReward = new RandomReward(5, 10, "Путешествие"),
            DifficultyLevel = 1,
            
            // === ПАРАМЕТРЫ БОЯ ===
            HasCombat = true,
            Combat = new CombatScenePart
            {
                EnemyName = "Лесной разбойник",
                EnemyMaxHealth = 10,
                EnemyDamageMin = 8,
                EnemyDamageMax = 16,
                EnemyArmor = 2,
                EnemyExperienceReward = 25,
                EnemyGoldReward = 15,
                NegotiationSuccessChance = 40,
                NegotiationSuccessText = "Разбойник опускает нож: 'Ладно, проходи.'",
                NegotiationFailText = "Разбойник смеётся: 'На смерть!' и бросается в атаку.",
                CombatWinNextSceneId = 3,
                CombatFleeNextSceneId = 3,
                Damage2EnemyReasons = new Dictionary<int, string>
                {
                    { 10, "Вы ударили Лесного разбойника кулаком" },
                    { 30, "Вы сильно ударили Лесного разбойника кулаком" },
                    { 60, "Вы ударили Лесного разбойника мечом" },
                    { 90, "Вы сильно ударили Лесного разбойника мечом" },
                },
                Damage2PlayerReasons = new Dictionary<int, string>
                {
                    { 20, "Вас ударил Лесной разбойник кулаком" },
                    { 30, "Вас сильно ударил Лесной разбойник кулаком" },
                    { 40, "Вас очень сильно ударил Лесной разбойник кулаком" },
                    { 80, "Вас невероятно сильно ударил Лесной разбойник кулаком" },
                }
            }
        },
        new()
        {
            Id = 3,
            Title = "Перекрёсток",
            Description = "Дорога расходится в три стороны. Куда пойдёте?",
            ToIds = new Dictionary<int, string>
            {
                {
                    0, "Вернуться в таверну"
                }
            },
            DifficultyLevel = 1,
            ExpReward = default,
            GoldReward = default,
            IsStoryQuest = false,
            Combat = default
        },
        new()
        {
            Id = 666,
            Title = "Лимбо",
            Description = "Вы попали в Лимбо. Вернитесь в таверну.",
            ToIds = new Dictionary<int, string>
            {
                {
                    0, "В таверну!"
                }
            },
            IsStoryQuest = false,
            DifficultyLevel = 0,
            ExpReward = default,
            GoldReward = default,
            Combat = default
        }
    ];
}