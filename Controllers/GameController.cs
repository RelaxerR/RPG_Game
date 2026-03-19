using Microsoft.AspNetCore.Mvc;
using RPG_Game.Services;

namespace RPG_Game.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly GameSessionService _sessionService;

    public GameController(GameSessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost("start")]
    public IActionResult StartGame([FromBody] string playerName)
    {
        if (string.IsNullOrEmpty(playerName)) return BadRequest("Имя не может быть пустым");

        var player = _sessionService.CreatePlayer(playerName);
        var firstQuest = _sessionService.GetNextQuest(player);

        return Ok(new {
            player = player,
            quest = firstQuest,
            message = $"Добро пожаловать, {player.Name}! Ваше приключение начинается..."
        });
    }
}