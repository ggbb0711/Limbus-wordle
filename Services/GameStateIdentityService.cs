using System.IO.Abstractions;
using System.Text.Json;
using Limbus_wordle.Interfaces;

namespace Limbus_wordle.Services
{
    public class GameStateIdentityService(IFileSystem fileSystem) : IGameStateService<Identity>
    {
        private readonly IFileSystem _fileSystem = fileSystem;
        private readonly Random _random = new Random();

        public async Task<GameState<Identity>> Guess(GameState<Identity> gameState, string guess)
        {
            if(gameState.GameOver) return gameState;
            
            var rootLink = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootLink, Environment.GetEnvironmentVariable("IdentityJSONFile"));
            string identitiesFile = await _fileSystem.File.ReadAllTextAsync(filePath);
            var deserializeIdentities = JsonSerializer.Deserialize<Dictionary<string,Identity>>(identitiesFile)
                ??new Dictionary<string,Identity>();
            var guessData = deserializeIdentities.Values.Where(i=>i.Name.Equals(guess)).FirstOrDefault();
            
            if(guessData==null) return gameState;
            
            if(gameState.CorrectGuess.Name.Equals(guess))
            {
                gameState.GuessCount += 1;
                gameState.HasWon = true;
                gameState.GameOver = true;
                gameState.Guesses = gameState.Guesses.Prepend(guessData).ToList();
                return gameState;
            }
            
            gameState.GuessCount += 1;
            gameState.Guesses = gameState.Guesses.Prepend(guessData).ToList();
            if(gameState.GuessCount>=gameState.MaxCount)
            { 
                gameState.HasWon = false;
                gameState.GameOver = true;
            }
            return gameState;
        }

        public async Task<GameState<Identity>?> GenerateNewGameState()
        {
            var rootLink = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootLink, Environment.GetEnvironmentVariable("IdentityJSONFile"));
            try
            {
                string identitiesFile = await _fileSystem.File.ReadAllTextAsync(filePath);
                var deserializeIdentities = JsonSerializer.Deserialize<Dictionary<string,Identity>>(identitiesFile)
                    ??new Dictionary<string,Identity>();
                
                var newGameState = new GameState<Identity>()
                {
                    CorrectGuess = deserializeIdentities.ElementAt(_random.Next(deserializeIdentities.Count))
                                    .Value,
                };
                return newGameState;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Cannot create new game");
                return null;
            }
        }
    }
}