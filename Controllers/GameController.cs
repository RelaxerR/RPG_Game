using Microsoft.AspNetCore.Mvc;
using RPG_Game.GameLogic.Engine;
using RPG_Game.GameLogic.Entities;
using RPG_Game.Services;

namespace RPG_Game.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(GameSessionService sessionService) : ControllerBase
{
    [HttpPost("start")]
    public IActionResult StartGame([FromBody] string playerName)
    {
        var player = sessionService.CreatePlayer(playerName);
        var firstQuest = sessionService.MoveToQuest(player.Name, 0, 0).Scene;
        return Ok(new {
            player = player,
            quest = firstQuest,
            events = new List<GameEvent>(),
            text = firstQuest.Description
        });
    }
    
    [HttpPost("action")]
    public IActionResult MakeAction([FromBody] ActionRequest request)
    {
        var result = sessionService.MoveToQuest(request.PlayerName, request.CurrentQuestId, request.TargetQuestId);
        return Ok(new {
            player = result.Player,
            quest = result.Scene,
            events = result.Events,
            text = result.Scene.Description
        });
    }
    
    // Специальный эндпоинт для боевых действий
    [HttpPost("combat")]
    public IActionResult CombatAction([FromBody] CombatRequest request)
    {
        var player = sessionService.GetPlayer(request.PlayerName);
        if (player == null || !player.IsAlive)
        {
            return BadRequest(new { error = "Player dead or not found" });
        }
        
        var currentQuest = QuestRepository.Quests.FirstOrDefault(q => q.Id == request.CurrentQuestId);
        if (currentQuest == null)
        {
            return BadRequest(new { error = "Quest not found" });
        }
        
        // -1 = Атака, -2 = Побег, -3 = Переговоры
        var result = sessionService.MoveToQuest(request.PlayerName, request.CurrentQuestId, request.ActionType);
        return Ok(new {
            player = result.Player,
            quest = result.Scene,
            events = result.Events,
            text = result.Scene.Description,
            combatState = player.ActiveCombat
        });
    }
}