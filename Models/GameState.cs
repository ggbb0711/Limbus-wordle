using Limbus_wordle.Interfaces;
public class GameState<Guess> : IGameState<Guess>
{
    public int GuessCount { get ; set ; } = 0;
    public Guess CorrectGuess { get; set ; } 
    public List<Guess> Guesses { get ; set ; } = [];
    public int MaxCount { get ; set; } = 7;
    public bool GameOver { get; set ; } = false;
    public bool HasWon { get ; set ; } = false;
}