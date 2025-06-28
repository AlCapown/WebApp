# nullable enable

using System.Collections.Immutable;

namespace WebApp.Server.Infrastructure;

public sealed class MicrosoftPublisherDomainModel
{
    private static readonly MicrosoftPublisherDomainModel _instance = new();

    private MicrosoftPublisherDomainModel() 
    {
        AssociatedApplications = ImmutableArray.Create
        ([
            new Application()
            {
                ApplicationId = "d0f40251-8829-4625-9129-cb8d1a0c3305"
            }
        ]);
    }

    public static MicrosoftPublisherDomainModel Value => _instance;

    public ImmutableArray<Application> AssociatedApplications { get; init; }

    public sealed record Application
    {
        public required string ApplicationId { get; init; }
    }
}