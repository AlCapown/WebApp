using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Common.Models;

public abstract record ErrorBase { }


[ImmutableObject(true)]
public sealed record ApiError : ErrorBase
{
    public string Title { get; init; }
    public int Status { get; init; }
    public string Detail { get; init; }
    public string TraceId { get; init; }
    public string StackTrace { get; init; }
    public ImmutableDictionary<string, ImmutableList<string>> Errors { get; init; }
    public string FetchName { get; init; }
    public object RetryAction { get; init; }
}

[ImmutableObject(true)]
public sealed record LocalError : ErrorBase
{
    public string Message { get; init; }
    public string StackTrace { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(ApiError))]
[JsonSerializable(typeof(LocalError))]
public partial class ErrorJsonContext : JsonSerializerContext { }

