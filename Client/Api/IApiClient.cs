using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Client.Api;

public interface IApiClient
{
    Task<ApiResponse<TResponse>> GetAsync<TResponse>(ApiLoadPlan<TResponse> apiLoadPlan, string uri)
        where TResponse : class;

    Task<ApiResponse<TResponse>> GetAsync<TResponse>(ApiLoadPlan<TResponse> apiLoadPlan, string uri, Dictionary<string, string> queryPrams)
        where TResponse : class;

    Task<ApiResponse<TResponse>> PostAsync<TResponse, TBody>(ApiLoadPlanWithBody<TResponse, TBody> apiLoadPlan, string uri, TBody body)
        where TResponse : class
        where TBody : class;

    Task<ApiResponse<TResponse>> PutAsync<TResponse, TBody>(ApiLoadPlanWithBody<TResponse, TBody> apiLoadPlan, string uri, TBody body)
        where TResponse : class
        where TBody : class;

    Task<ApiResponse<TResponse>> DeleteAsync<TResponse>(ApiLoadPlan<TResponse> apiLoadPlan, string uri)
        where TResponse : class;
}
