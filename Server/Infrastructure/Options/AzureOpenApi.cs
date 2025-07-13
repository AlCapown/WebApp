#nullable enable

namespace WebApp.Server.Infrastructure.Options;

public record AzureOpenAI
{
    public string? DeploymentName { get; init; }
    public string? Endpoint { get; init; }
    public string? ApiKey { get; init; }
}
