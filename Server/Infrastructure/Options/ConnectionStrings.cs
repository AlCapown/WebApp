using System.ComponentModel.DataAnnotations;

namespace WebApp.Server.Infrastructure.Options;

public sealed record ConnectionStrings
{
    [Required]
    public required string Database { get; init; }

    [Required]
    public required string HangfireDatabase { get; init; }

    [Required]
    public required string Redis { get; init; }
}