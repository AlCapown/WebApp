#nullable enable

using System.Text.Json.Serialization.Metadata;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Api;

public abstract class ApiLoadPlan<TResponse>
    where TResponse : class
{
    public FetchStartedAction FetchStartedAction { get; }

    public abstract JsonTypeInfo<TResponse> ResponseJsonContext { get; }

    protected ApiLoadPlan(FetchStartedAction fetchStartedAction)
    {
        FetchStartedAction = fetchStartedAction;
    }

    public abstract FetchSuccessAction GetSuccessAction(TResponse response);
    public abstract FetchFailureAction GetFailureAction(ApiError apiError);
}

public abstract class ApiLoadPlanWithBody<TResponse, TBody> : ApiLoadPlan<TResponse>
    where TResponse : class
    where TBody : class
{
    public abstract JsonTypeInfo<TBody> BodyJsonContext { get; }

    protected ApiLoadPlanWithBody(FetchStartedAction fetchStartedAction) : base(fetchStartedAction) { }
}

