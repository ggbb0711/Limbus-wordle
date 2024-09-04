using Limbus_wordle.Interfaces;

namespace Limbus_wordle.Models
{
    public class DailyGameMode<GuessType> : IGameMode<GameState<GuessType>>
    {
        public int WinStreak { get; set; } = 0;
        public int BestScore { get; set; } = 0;
        public DateTime GuessDate { get; set; } = DateTime.Now;
        public GameState<GuessType> GameState { get; set; }
    }
}