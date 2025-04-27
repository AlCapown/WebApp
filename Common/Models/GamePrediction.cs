using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Common.Models;

public record GamePrediction
{
    public int GamePredictionId { get; init; }
    public int SeasonId { get; init; }
    public int GameId { get; init; }
    public bool IsCurrentUser { get; init; }
    public string UserId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public int HomeTeamId { get; init; }
    public int PredictedHomeTeamScore { get; init; }
    public int AwayTeamId { get; init; }
    public int PredictedAwayTeamScore { get; init; }
}

public record GetGamePredictionResponse
{
    public GamePrediction GamePrediction { get; init; }
}

public record GamePredictionSearchResponse
{
    public GamePrediction[] GamePredictions { get; init; }
}

public class CreateGamePredictionRequest
{
    [Required]
    public int? GameId { get; set; }

    [Required]
    public int? HomeTeamScore { get; set; }

    [Required]
    public int? AwayTeamScore { get; set; }
}

public record CreateGamePredictionResponse
{
    public int GamePredictionId { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(GetGamePredictionResponse))]
public partial class GetGamePredictionResponseJsonContext : JsonSerializerContext { }

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(GamePredictionSearchResponse))]
public partial class GamePredictionSearchResponseJsonContext : JsonSerializerContext { }

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(CreateGamePredictionRequest))]
public partial class CreateGamePredictionRequestJsonContext : JsonSerializerContext { }

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(CreateGamePredictionResponse))]
public partial class CreateGamePredictionResponseJsonContext : JsonSerializerContext { }