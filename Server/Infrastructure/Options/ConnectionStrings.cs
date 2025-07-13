#nullable enable

namespace WebApp.Server.Infrastructure.Options;

public record ConnectionStrings
{
    public string? Database { get; init; }
    public string? HangfireDatabase { get; init; }
    public string? Redis { get; init; }
}