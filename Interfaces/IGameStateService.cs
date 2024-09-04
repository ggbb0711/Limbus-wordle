namespace Limbus_wordle.Interfaces
{
    public interface IGameStateService<GuessType>
    {
        Task<GameState<GuessType>> Guess(GameState<GuessType> gameState,string guess);
        Task<GameState<Identity>?> GenerateNewGameState();
    }
}