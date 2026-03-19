using Microsoft.AspNetCore.Mvc;
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
        var firstQuest = sessionService.MoveToQuest(player.Name, 0).Quest;

        // Возвращаем объект в том же формате, что и ActionResultDto
        return Ok(new {
            player = player,
            quest = firstQuest,
            events = new List<GameEvent>(), // Пустой список, чтобы не было undefined
            text = firstQuest.Description 
        });
    }

    
    [HttpPost("action")]
    public IActionResult MakeAction([FromBody] ActionRequest request)
    {
        // ActionRequest содержит PlayerName и TargetQuestId
        var nextQuest = sessionService.MoveToQuest(request.PlayerName, request.TargetQuestId).Quest;
        var player = sessionService.GetPlayer(request.PlayerName);

        return Ok(new {
            player = player,
            quest = nextQuest,
            // Позже сюда добавим сгенерированный Qwen текст
            text = nextQuest.Description 
        });
    }

}