namespace Limbus_wordle.Interfaces
{
    public interface IGameMode<GameStateType>
    {
        int WinStreak { get; set;}
        int BestScore { get; set;}
        GameStateType GameState { get; set; }
    }
}