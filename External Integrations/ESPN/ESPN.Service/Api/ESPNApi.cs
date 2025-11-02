using System.Net.Http.Json;
using WebApp.ExternalIntegrations.ESPN.Service.Models;

namespace WebApp.ExternalIntegrations.ESPN.Service.Api;

public sealed class ESPNApi : IESPNApi
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ESPNApi(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ESPNScoreboardModel> GetScoreboardAsync(CancellationToken token)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.ESPN_SERVICE_NAME);

        return await httpClient.GetFromJsonAsync(
            requestUri: "https://site.api.espn.com/apis/site/v2/sports/football/nfl/scoreboard",
            jsonTypeInfo: ESPNScoreboardModelJsonContext.Default.ESPNScoreboardModel,
            cancellationToken: token);
    }
}


