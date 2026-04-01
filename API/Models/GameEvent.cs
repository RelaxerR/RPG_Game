namespace RPG_Game.API.Models;

public readonly record struct GameEvent(string message, GameEventType gameEventType = GameEventType.Info)
{
    public override string ToString() => $"{{ message = {message}, gameEventType = {gameEventType} }}";
}