using Limbus_wordle.Interfaces;

namespace Limbus_wordle.Models
{
    public class EndlessGameMode<GuessType> : IGameMode<GameState<GuessType>>
    {
        public int WinStreak { get; set; } = 0;
        public int BestScore { get; set; } = 0;
        public GameState<GuessType> GameState { get; set; }
    }
}