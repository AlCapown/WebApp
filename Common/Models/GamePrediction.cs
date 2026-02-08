using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Common.Models;

public sealed record GamePrediction
{
    public int GamePredictionId { get; init; }
    public int SeasonId { get; init; }
    public int SeasonWeekId { get; init; }
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

public sealed record GetGamePredictionResponse
{
    public GamePrediction GamePrediction { get; init; }
}

public sealed record GamePredictionSearchResponse
{
    public GamePrediction[] GamePredictions { get; init; }
}

public sealed class CreateGamePredictionRequest
{
    [Required]
    public int? GameId { get; set; }

    [Required]
    public int? HomeTeamScore { get; set; }

    [Required]
    public int? AwayTeamScore { get; set; }
}

public sealed record CreateGamePredictionResponse
{
    public int GamePredictionId { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(GetGamePredictionResponse))]
public sealed partial class GetGamePredictionResponseJsonContext : JsonSerializerContext { }

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(GamePredictionSearchResponse))]
public sealed partial class GamePredictionSearchResponseJsonContext : JsonSerializerContext { }

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(CreateGamePredictionRequest))]
public sealed partial class CreateGamePredictionRequestJsonContext : JsonSerializerContext { }

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(CreateGamePredictionResponse))]
public sealed partial class CreateGamePredictionResponseJsonContext : JsonSerializerContext { }