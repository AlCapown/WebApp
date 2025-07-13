#nullable enable

namespace WebApp.Server.Infrastructure.Options;

public record Authentication
{
    public required MicrosoftOptions Microsoft { get; init; }
    public required GoogleOptions Google { get; init; }
    public required YahooOptions Yahoo { get; init; }
}

public record MicrosoftOptions
{
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
    public string? AuthorizationEndpoint { get; init; }
    public string? TokenEndpoint { get; init; }
}

public record GoogleOptions
{
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
    public string? AuthorizationEndpoint { get; init; }
    public string? TokenEndpoint { get; init; }
}

public record YahooOptions
{
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
    public string? AuthorityEndpoint { get; init; }
}
