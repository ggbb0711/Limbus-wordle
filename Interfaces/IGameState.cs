namespace Limbus_wordle.Interfaces
{
    public interface IGameState<Guess>
    {
        int GuessCount { get; set;}
        int MaxCount { get; set;}
        bool HasWon { get; set; }
        bool GameOver { get; set; }
        Guess CorrectGuess { get; set; }
        List<Guess> Guesses { get; set; }
    }
}