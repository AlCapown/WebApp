using WebApp.ExternalIntegrations.ESPN.Service.Models;

namespace WebApp.ExternalIntegrations.ESPN.Service.Api;

public interface IESPNApi
{
    Task<ESPNScoreboardModel> GetScoreboardAsync(CancellationToken token);
}
