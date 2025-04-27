using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Extensions;
using WebApp.ExternalIntegrations.NFL.Service.Models;

namespace WebApp.ExternalIntegrations.NFL.Service.Api
{
    public class NFLApi : INFLApi
    {
        private readonly HttpClient _client;

        private static readonly TimeZoneInfo _tz = 
            TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public NFLApi(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient(Constants.NFL_SERVICE_NAME);
        }

        public async Task<WeekResult> GetWeekScoreResults(int seasonYear, SeasonType seasonType, int week)
        {
            string url = QueryHelpers.AddQueryString("https://static.nfl.com/ajax/scorestrip", new Dictionary<string, string>
            {
                { "season", seasonYear.ToString() },
                { "seasonType", seasonType.ToShortName() },
                { "week", week.ToString() }
            });

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            string response = await SendRequest(requestMessage);

            var result = response.XmlDeserializeFromString<WeekResult>();

            if(result?.Week?.Games == null)
            {
                return result;
            }

            foreach(var game in result.Week.Games)
            {
                game.StartsOn = Helpers.ParseGameStartDateTime(game.Eid, game.Time, _tz);
            }

            return result;
        }

        private async Task<string> SendRequest(HttpRequestMessage message)
        {
            var result = await _client.SendAsync(message);
            if(result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }

            // TODO add better exception handling
            throw new Exception($"The request to the NFL api returned with the following failure status code ${result.StatusCode}");
        }

    }
}
