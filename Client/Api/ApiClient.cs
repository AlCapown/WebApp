using Fluxor;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApp.Client.Common.Constants;
using WebApp.Client.Store.FetchStore;
using WebApp.Client.Store.PageStore;
using WebApp.Common.Infrastructure;
using WebApp.Common.Models;

namespace WebApp.Client.Api;

public sealed class ApiClient : IApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDispatcher _dispatcher;

    public ApiClient(IHttpClientFactory httpClientFactory, IDispatcher dispatcher)
    {
        _httpClientFactory = httpClientFactory;
        _dispatcher = dispatcher;
    }

    #region public methods

    public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(ApiLoadPlan<TResponse> apiLoadPlan, string uri)
        where TResponse : class
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceConstants.WEBAPP_API_CLIENT);

            var response = await httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                return await HandleSuccess(apiLoadPlan, response);
            }

            return await HandleApiFailure(apiLoadPlan, response);
        }
        catch (Exception ex)
        {
            return HandleException(apiLoadPlan, ex);
        }
    }


    public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(ApiLoadPlan<TResponse> apiLoadPlan, string uri, Dictionary<string, string> queryPrams)
        where TResponse : class
        => await GetAsync(apiLoadPlan, QueryHelpers.AddQueryString(uri, queryPrams));


    public async Task<ApiResponse<TResponse>> PostAsync<TResponse, TBody>(ApiLoadPlanWithBody<TResponse, TBody> apiLoadPlan, string uri, TBody body)
        where TResponse : class
        where TBody : class
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceConstants.WEBAPP_API_CLIENT);

            var response = await httpClient.PostAsJsonAsync(uri, body, apiLoadPlan.BodyJsonContext);

            if (response.IsSuccessStatusCode)
            {
                return await HandleSuccess(apiLoadPlan, response);
            }

            return await HandleApiFailure(apiLoadPlan, response);
        }
        catch (Exception ex)
        {
            return HandleException(apiLoadPlan, ex);
        }
    }


    public async Task<ApiResponse<TResponse>> PutAsync<TResponse, TBody>(ApiLoadPlanWithBody<TResponse, TBody> apiLoadPlan, string uri, TBody body)
        where TResponse : class
        where TBody : class
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceConstants.WEBAPP_API_CLIENT);

            var response = await httpClient.PutAsJsonAsync(uri, body, apiLoadPlan.BodyJsonContext);

            if (response.IsSuccessStatusCode)
            {
                return await HandleSuccess(apiLoadPlan, response);
            }

            return await HandleApiFailure(apiLoadPlan, response);
        }
        catch (Exception ex)
        {
            return HandleException(apiLoadPlan, ex);
        }
    }


    public async Task<ApiResponse<TResponse>> DeleteAsync<TResponse>(ApiLoadPlan<TResponse> apiLoadPlan, string uri)
        where TResponse : class
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceConstants.WEBAPP_API_CLIENT);

            var response = await httpClient.DeleteAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                return await HandleSuccess(apiLoadPlan, response);
            }

            return await HandleApiFailure(apiLoadPlan, response);
        }
        catch (Exception ex)
        {
            return HandleException(apiLoadPlan, ex);
        }
    }

    #endregion

    #region Private Methods

    private async ValueTask<ApiResponse<TResponse>> HandleSuccess<TResponse>(ApiLoadPlan<TResponse> apiLoadPlan, HttpResponseMessage response)
        where TResponse : class
    {
        TResponse content = typeof(TResponse) == typeof(NoContentResponse)
            ? NoContentResponse.Value as TResponse
            : await response.Content.ReadFromJsonAsync(apiLoadPlan.ResponseJsonContext);

        _dispatcher.Dispatch(apiLoadPlan.GetSuccessAction(content));

        _dispatcher.Dispatch(new FetchActions.FetchSuccess
        {
            FetchName = apiLoadPlan.FetchStartedAction.FetchName,
            CacheExpires = DateTimeOffset.Now.AddMinutes(apiLoadPlan.FetchStartedAction.CacheDurationInMinutes)
        });

        return new ApiResponse<TResponse>
        {
            IsSuccess = true,
            StatusCode = (int)response.StatusCode,
            Response = content,
        };
    }


    private async Task<ApiResponse<TResponse>> HandleApiFailure<TResponse>(ApiLoadPlan<TResponse> apiLoadPlan, HttpResponseMessage response)
        where TResponse : class
    {
        var apiError = await GetApiErrorFromResponse(response);
        return DispatchFailureAndReturnResponse(apiLoadPlan, apiError);
    }


    private ApiResponse<TResponse> HandleException<TResponse>(ApiLoadPlan<TResponse> apiLoadPlan, Exception exception)
        where TResponse : class
    {
        var apiError = GetApiErrorFromException(exception);
        return DispatchFailureAndReturnResponse(apiLoadPlan, apiError);
    }

    private ApiResponse<TResponse> DispatchFailureAndReturnResponse<TResponse>(ApiLoadPlan<TResponse> apiLoadPlan, ApiError apiError)
        where TResponse : class
    {
        _dispatcher.Dispatch(apiLoadPlan.GetFailureAction(apiError) with
        {
            FetchName = apiLoadPlan.FetchStartedAction.FetchName,
            ApiError = apiError
        });

        _dispatcher.Dispatch(new FetchActions.FetchFailure
        {
            FetchName = apiLoadPlan.FetchStartedAction.FetchName,
            ApiError = apiError
        });

        if (apiLoadPlan.FetchStartedAction.DispatchErrorToWindow)
        {
            _dispatcher.Dispatch(new PageActions.EnqueuePageError
            {
                Error = new ApiError
                {
                    FetchName = apiLoadPlan.FetchStartedAction.FetchName,
                    Message = apiError.Message,
                    StatusCode = apiError.StatusCode,
                    RetryAction = apiLoadPlan.FetchStartedAction
                }
            });
        }

        return new ApiResponse<TResponse>
        {
            IsSuccess = false,
            StatusCode = apiError.StatusCode,
            Response = null
        };
    }

    private static async ValueTask<ApiError> GetApiErrorFromResponse(HttpResponseMessage response)
    {
        try
        {
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest or 
                HttpStatusCode.Forbidden or 
                HttpStatusCode.NotFound or 
                HttpStatusCode.Conflict or
                HttpStatusCode.InternalServerError or 
                HttpStatusCode.ServiceUnavailable => await response.Content.ReadFromJsonAsync(ApiErrorJsonContext.Default.ApiError),
                HttpStatusCode.Unauthorized => new ApiError
                {
                    Message = "Unauthorized",
                    StatusCode = (int)response.StatusCode
                },
                _ => UnknownApiErrorOccurred((int)response.StatusCode),
            };
        }
        catch
        {
            return UnknownApiErrorOccurred((int)response.StatusCode);
        }
    }

    private static ApiError UnknownApiErrorOccurred(int statusCode) => new()
    {
        Message = "An unknown error occurred.",
        StatusCode = statusCode
    };

    private static ApiError GetApiErrorFromException(Exception exception) => new()
    {
        Message = exception.Message,
        StatusCode = 500,
    };

    #endregion
}
