using WebApp.ExternalIntegrations.NFL.Service.Models;
using System.Threading.Tasks;
using WebApp.Common.Constants;

namespace WebApp.ExternalIntegrations.NFL.Service.Api
{
    public interface INFLApi
    {
        Task<WeekResult> GetWeekScoreResults(int seasonYear, SeasonType seasonType, int week);
    }
}
