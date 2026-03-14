using System.ComponentModel.DataAnnotations;

namespace WebApp.Server.Infrastructure.Options;

public sealed record Authentication
{
    [Required]
    public required MicrosoftOptions Microsoft { get; init; }

    [Required]
    public required GoogleOptions Google { get; init; }

    [Required]
    public required YahooOptions Yahoo { get; init; }
}

public sealed record MicrosoftOptions
{
    [Required]
    public required string ClientId { get; init; }

    [Required]
    public required string ClientSecret { get; init; }

    [Required]
    public required string AuthorizationEndpoint { get; init; }

    [Required]
    public required string TokenEndpoint { get; init; }
}

public sealed record GoogleOptions
{
    [Required]
    public required string ClientId { get; init; }

    [Required]
    public required string ClientSecret { get; init; }

    [Required]
    public required string AuthorizationEndpoint { get; init; }

    [Required]
    public required string TokenEndpoint { get; init; }
}

public sealed record YahooOptions
{
    [Required]
    public required string ClientId { get; init; }

    [Required]
    public required string ClientSecret { get; init; }

    [Required]
    public required string AuthorityEndpoint { get; init; }
}
