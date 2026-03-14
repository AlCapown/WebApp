using System.ComponentModel.DataAnnotations;

namespace WebApp.Server.Infrastructure.Options;

public sealed record AzureOpenAI
{
    [Required]
    public required string DeploymentName { get; init; }

    [Required]
    public required string Endpoint { get; init; }

    [Required]
    public required string ApiKey { get; init; }
}
