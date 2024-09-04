

// using System.IO.Abstractions;
// using System.Text.Json;
// using dotenv.net;
// using Limbus_wordle.Controllers;
// using Limbus_wordle.Models;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using Xunit;

// public class HomeControllerTest
// {
//     private readonly Mock<ILogger<HomeController>> _loggerMock;
//     private readonly Mock<IFileSystem> _fileSystemMock;
//     private readonly Mock<Random> _randomMock;
//     private readonly HomeController _controller;
//     public HomeControllerTest()
//     {
//         DotEnv.Load();
//         Environment.SetEnvironmentVariable("IdentityJSONFile", "Folder/Models.json");
//         _loggerMock = new Mock<ILogger<HomeController>>();
//         _fileSystemMock = new Mock<IFileSystem>();
//         _randomMock = new Mock<Random>();
//         _controller = new HomeController(_loggerMock.Object,_fileSystemMock.Object, _randomMock.Object);
//         var httpContext = new DefaultHttpContext();
//         var controllerContext = new ControllerContext()
//         {
//             HttpContext = httpContext,
//         };

//         _controller.ControllerContext = controllerContext;
//     }

//     private void SetupGameState(GameState<Identity> gameState)
//     {
//         _controller.ViewBag.GameState = gameState;
//     }


//     [Fact]
//     public async Task AutoComplete_ReturnsMatchingIdentities()
//     {
//         // Arrange
//         var identities = new Dictionary<string, Identity>
//         {
//             { "1", new Identity { Name = "TestIdentity1" } },
//             { "2", new Identity { Name = "SampleIdentity2" } }
//         };

//         var identitiesJson = JsonSerializer.Serialize(identities);

//         // Mocking System.IO.File.ReadAllTextAsync method
//         _fileSystemMock.Setup(fs => fs.File.ReadAllTextAsync(It.IsAny<string>(),CancellationToken.None))
//                       .ReturnsAsync(identitiesJson);

//         // Act
//         var result = await _controller.AutoComplete("Test");
//         var resultData = result.Value as ResponseService<List<Identity>>;
//         // Assert
//         Assert.NotNull(resultData);
//         Assert.Single(resultData.Response);
//         Assert.Equal("TestIdentity1", resultData.Response[0].Name);
//     }

//     [Fact]
//     public async Task AutoComplete_ReturnsEmptyListWhenNoMatch()
//     {
//         // Arrange
//         var identities = new Dictionary<string, Identity>
//         {
//             { "1", new Identity { Name = "TestIdentity1" } },
//             { "2", new Identity { Name = "SampleIdentity2" } }
//         };

//         var identitiesJson = JsonSerializer.Serialize(identities);

//         // Mocking System.IO.File.ReadAllTextAsync method
//         _fileSystemMock.Setup(fs => fs.File.ReadAllTextAsync(It.IsAny<string>(),CancellationToken.None))
//                       .ReturnsAsync(identitiesJson);


//         // Act
//         var result = await _controller.AutoComplete("NoMatch");
//         var resultData = result.Value as ResponseService<List<Identity>>;

//         // Assert
//         Assert.NotNull(resultData);
//         Assert.Empty(resultData.Response);
//     }

//     [Fact]
//     public async Task AutoComplete_ReturnsAllWhenTermIsEmpty()
//     {
//         // Arrange
//         var identities = new Dictionary<string, Identity>
//         {
//             { "1", new Identity { Name = "TestIdentity1" } },
//             { "2", new Identity { Name = "SampleIdentity2" } }
//         };

//         var identitiesJson = JsonSerializer.Serialize(identities);

//         // Mocking System.IO.File.ReadAllTextAsync method
//         _fileSystemMock.Setup(fs => fs.File.ReadAllTextAsync(It.IsAny<string>(),CancellationToken.None))
//                       .ReturnsAsync(identitiesJson);

//         // Act
//         var result = await _controller.AutoComplete("");
//         var resultData = result.Value as ResponseService<List<Identity>>;

//         // Assert
//         Assert.NotNull(resultData);
//         Assert.Equal(2, resultData.Response.Count);
//     }

//     [Fact]
//     public async Task AutoComplete_ReturnsEmptyListWhenFileReadFails()
//     {
//         // Arrange

//         // Mocking System.IO.File.ReadAllTextAsync method to throw an exception
//         _fileSystemMock.Setup(fs => fs.File.ReadAllTextAsync(It.IsAny<string>(),CancellationToken.None))
//                       .ThrowsAsync(new IOException("File read error"));

//         // Act
//         var result = await _controller.AutoComplete("Test");
//         var resultData = result.Value as ResponseService<List<Identity>>;

//         // Assert
//         Assert.NotNull(resultData);
//         Assert.Empty(resultData.Response);
//     }

//     [Fact]
//     public async Task Guess_GameStateNotFound_ReturnsBadRequest()
//     {
//         // Arrange
//         SetupGameState(null);

//         // Act
//         var result = await _controller.Guess("SomeGuess");
//         var jsonResult = Assert.IsType<JsonResult>(result);
//         var response = Assert.IsType<ResponseService<Identity>>(jsonResult.Value);

//         // Assert
//         Assert.Equal("Cannot find the game state", response.Msg);
//         Assert.Equal(400, _controller.Response.StatusCode);
//     }

//     [Fact]
//     public async Task Guess_GameAlreadyEnded_ReturnsBadRequest()
//     {
//         // Arrange
//         var gameState = new GameState<Identity>
//         {
//             CorrectGuess = new Identity { Name = "CorrectGuess" },
//             GuessCount = 5,
//             MaxCount = 5,
//             Guesses = new List<Identity>()
//         };
//         SetupGameState(gameState);

//         // Act
//         var result = await _controller.Guess("SomeGuess");
//         var jsonResult = Assert.IsType<JsonResult>(result);
//         var response = Assert.IsType<ResponseService<Identity>>(jsonResult.Value);

//         // Assert
//         Assert.Equal("Game has already ended. Started a new one", response.Msg);
//         Assert.Equal(400, _controller.Response.StatusCode);
//     }

//     [Fact]
//     public async Task Guess_GuessNotFound_ReturnsNotFound()
//     {
//         // Arrange
//         var gameState = new GameState<Identity>
//         {
//             CorrectGuess = new Identity { Name = "CorrectGuess" },
//             GuessCount = 0,
//             MaxCount = 5,
//             Guesses = new List<Identity>()
//         };
//         SetupGameState(gameState);

//         var identitiesJson = "{}"; // No identities in JSON
//         _fileSystemMock.Setup(fs => fs.File.ReadAllTextAsync(It.IsAny<string>(),CancellationToken.None))
//                     .ReturnsAsync(identitiesJson);

//         // Act
//         var result = await _controller.Guess("NonExistingGuess");
//         var jsonResult = Assert.IsType<JsonResult>(result);
//         var response = Assert.IsType<ResponseService<Identity>>(jsonResult.Value);

//         // Assert
//         Assert.Equal("The guess doesn't exist in the system", response.Msg);
//         Assert.Equal(404, _controller.Response.StatusCode);
//     }

//     [Fact]
//     public async Task Guess_CorrectGuess_ReturnsUpdatedGameState()
//     {
//         // Arrange
//         var gameState = new GameState<Identity>
//         {
//             CorrectGuess = new Identity { Name = "CorrectGuess" },
//             GuessCount = 0,
//             MaxCount = 5,
//             Guesses = new List<Identity>()
//         };
//         SetupGameState(gameState);

//         var identities = new Dictionary<string, Identity>
//         {
//             { "1", new Identity { Name = "CorrectGuess" } }
//         };
//         var identitiesJson = JsonSerializer.Serialize(identities);
//         _fileSystemMock.Setup(fs => fs.File.ReadAllTextAsync(It.IsAny<string>(),CancellationToken.None))
//                     .ReturnsAsync(identitiesJson);

//         // Act
//         var result = await _controller.Guess("CorrectGuess");
//         var jsonResult = Assert.IsType<JsonResult>(result);
//         var response = Assert.IsType<ResponseService<Identity>>(jsonResult.Value);

//         // Assert
//         Assert.True(gameState.HasWon);
//         Assert.NotNull(response.Response);
//         Assert.Equal("CorrectGuess", response.Response.Name);
//     }

//     [Fact]
//     public async Task Guess_IncorrectGuess_ReturnsUpdatedGameState()
//     {
//         // Arrange
//         var gameState = new GameState<Identity>
//         {
//             CorrectGuess = new Identity { Name = "CorrectGuess" },
//             GuessCount = 0,
//             MaxCount = 5,
//             Guesses = new List<Identity>()
//         };
//         SetupGameState(gameState);

//         var identities = new Dictionary<string, Identity>
//         {
//             { "1", new Identity { Name = "IncorrectGuess" } }
//         };
//         var identitiesJson = JsonSerializer.Serialize(identities);
//         _fileSystemMock.Setup(fs => fs.File.ReadAllTextAsync(It.IsAny<string>(),CancellationToken.None))
//                     .ReturnsAsync(identitiesJson);

//         // Act
//         var result = await _controller.Guess("IncorrectGuess");
//         var jsonResult = Assert.IsType<JsonResult>(result);
//         var response = Assert.IsType<ResponseService<Identity>>(jsonResult.Value);

//         // Assert
//         Assert.False(gameState.HasWon);
//         Assert.NotNull(response.Response);
//         Assert.Equal("IncorrectGuess", response.Response.Name);
//     }

//     [Fact]
//     public async Task Guess_ExceptionThrown_ReturnsServerError()
//     {
//         // Arrange
//         var gameState = new GameState<Identity>
//         {
//             CorrectGuess = new Identity { Name = "CorrectGuess" },
//             GuessCount = 0,
//             MaxCount = 5,
//             Guesses = new List<Identity>()
//         };
//         SetupGameState(gameState);

//         _fileSystemMock.Setup(fs => fs.File.ReadAllTextAsync(It.IsAny<string>(),CancellationToken.None))
//                     .ThrowsAsync(new Exception("Test exception"));

//         // Act
//         var result = await _controller.Guess("CorrectGuess");
//         var jsonResult = Assert.IsType<JsonResult>(result);
//         var response = Assert.IsType<ResponseService<Identity>>(jsonResult.Value);

//         // Assert
//         Assert.Equal(500, _controller.Response.StatusCode);
//     }

// }