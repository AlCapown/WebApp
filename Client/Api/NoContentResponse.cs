#nullable enable

using System;

namespace WebApp.Client.Api;

/// <summary>
/// Represents a http 204 no content response.
/// </summary>
public sealed class NoContentResponse : IEquatable<NoContentResponse>, IComparable<NoContentResponse>, IComparable
{
    private static readonly NoContentResponse _instance = new();
    private NoContentResponse() { }
    public static NoContentResponse Value => _instance;
    public static bool operator ==(NoContentResponse? _, NoContentResponse? __) => true;
    public static bool operator !=(NoContentResponse? _, NoContentResponse? __) => false;
    public override string ToString() => nameof(NoContentResponse);
    public override int GetHashCode() => 0;
    public override bool Equals(object? obj) => obj is NoContentResponse;
    public bool Equals(NoContentResponse? other) => other is not null;
    public int CompareTo(NoContentResponse? other) => 0;
    public int CompareTo(object? obj) => obj is NoContentResponse ? 0 : 1;
}
