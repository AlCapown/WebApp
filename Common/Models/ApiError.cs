using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Common.Models;

public record ErrorBase { }

public sealed record ApiError : ErrorBase
{
    public string Message { get; init; }
    public int StatusCode { get; init; }
    public ImmutableDictionary<string, ImmutableList<string>> Errors { get; init; }
    public string FetchName { get; init; }
    public object RetryAction { get; init; }
    public string TraceId { get; init; }
}

public sealed record LocalError : ErrorBase
{
    public Type TypeOfException { get; init; }
    public string Message { get; init; }
    public string StackTrace { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(ApiError))]
public partial class ApiErrorJsonContext : JsonSerializerContext { }

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(LocalError))]
public partial class LocalErrorJsonContext : JsonSerializerContext { }