using Microsoft.AspNetCore.Mvc;
using RPG_Game.API.Models;
using RPG_Game.GameLogic.Engine;
using RPG_Game.GameLogic.Entities;
using RPG_Game.Services;

namespace RPG_Game.API.Controllers;

[ApiController]
[Route(ApiController)]
public class GameController(GameSessionService sessionService, ILogger<GameController> logger) : ControllerBase
{
    #region API constants

    private const string ApiController = "api/[controller]";
    private const string Start = "start";
    private const string Action = "action";
    private const string Combat = "combat";
    
    #endregion

    #region ApiRequests

    [HttpPost(Start)]
    public IActionResult StartGame([FromBody] string playerName)
    {
        logger.LogInformation("Starting game {PlayerName}", playerName);
        
        if (string.IsNullOrWhiteSpace(playerName))
        {
            return BadRequest(new StartGameBadRequest());
        }
        
        var player = sessionService.GetOrCreatePlayer(playerName);
        var result = sessionService.MoveToQuest(player, 0);
        
        logger.LogInformation("Game started for {PlayerName}, first quest: {QuestId} (OK)", playerName, result.Scene.Id);
        
        return Ok(new StartGameOk(player, result.Scene, result.Events, result.Scene.Description));
    }

    [HttpPost(Action)]
    public IActionResult MakeAction([FromBody] ActionRequest request)
    {
        logger.LogInformation("Making action {Action}", request);
        
        var player = sessionService.GetOrCreatePlayer(request.PlayerName);
        var result = sessionService.MoveToQuest(player, request.TargetQuestId);
        
        if (player is not { IsAlive: true } || !QuestsById.TryGetValue(request.CurrentQuestId, out _))
        {
            logger.LogWarning("No quest found for {PlayerName} or player dead (BAD)", request.PlayerName);
            return BadRequest(new ActionBadRequest());
        }
        
        logger.LogInformation("Game started for {PlayerName}", request.PlayerName);
        
        return Ok(new MakeActionOk(result.Player, result.Scene, result.Events, result.Scene.Description, player.ActiveCombat));
    }

    [HttpPost(Combat)]
    public IActionResult CombatAction([FromBody] CombatRequest request)
    {
        logger.LogInformation("Making action {Action}", request);
        
        var player = sessionService.GetOrCreatePlayer(request.PlayerName);
        if (player is not { IsAlive: true } || !QuestsById.TryGetValue(request.CurrentQuestId, out _))
        {
            logger.LogWarning("No quest found for {PlayerName} or player dead (BAD)", request.PlayerName);
            return BadRequest(new CombatBadRequest());
        }

        // -1 = Атака, -2 = Побег, -3 = Переговоры
        var result = sessionService.HandleCombatAction(player, request.CurrentQuestId, request.ActionType, []);
        
        logger.LogInformation("Game started for {PlayerName}", request.PlayerName);
        
        return Ok(new CombatActionOk(result.Player, result.Scene, result.Events, result.Scene.Description, player.ActiveCombat));
    }

    #endregion

    #region Private fields

    private readonly record struct StartGameBadRequest;
    private readonly record struct CombatBadRequest;
    private readonly record struct ActionBadRequest;
    
    private readonly record struct StartGameOk(Player player, Scene quest, List<GameEvent> events, string text)
    {
        public override string ToString() => $"{{ player = {player}, quest = {quest}, events = {events}, text = {text} }}";
    }
    private readonly record struct  MakeActionOk(Player player, Scene quest, List<GameEvent> events, string text, CombatState? combatState)
    {
        public override string ToString() => $"{{ player = {player}, quest = {quest}, events = {events}, text = {text}, combatState = {combatState} }}";
    }
    private readonly record struct CombatActionOk(Player player, Scene quest, List<GameEvent> events, string text, CombatState? combatState)
    {
        public override string ToString() => $"{{ player = {player}, quest = {quest}, events = {events}, text = {text}, combatState = {combatState} }}";
    }
    
    private static readonly Dictionary<int, Scene> QuestsById = QuestRepository.Quests.ToDictionary(q => q.Id);    

    #endregion
}