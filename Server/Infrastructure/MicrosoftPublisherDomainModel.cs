using System.Collections.Immutable;

namespace WebApp.Server.Infrastructure;

internal sealed class MicrosoftPublisherDomainModel
{
    private static readonly MicrosoftPublisherDomainModel _instance = new();

    public ImmutableArray<Application> AssociatedApplications { get; }

    private MicrosoftPublisherDomainModel()
    {
        AssociatedApplications =
        [
            new Application("d0f40251-8829-4625-9129-cb8d1a0c3305")
        ];
    }

    public static MicrosoftPublisherDomainModel Instance => _instance;

    internal sealed record Application(string ApplicationId);
}