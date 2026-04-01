namespace RPG_Game.GameLogic.Entities;

public struct RandomReward(int min, int max, string reason)
{
    private static readonly Random _rnd = new Random();

    private readonly int Min = min;
    private readonly int Max = max;
    public string Reason { get; set; } = reason;
    
    public int GetReward() => Max > Min ? _rnd.Next(Min, Max) : 0;
}
