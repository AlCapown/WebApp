#nullable enable

using System;
using System.Text.Json.Serialization.Metadata;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Api;

/// <summary>
/// Abstract base class for defining API request plans that handle HTTP operations with typed responses.
/// This class encapsulates the configuration needed for API calls including JSON serialization contexts,
/// action dispatching, and success/failure handling within the Fluxor state management pattern.
/// </summary>
/// <typeparam name="TResponse">The type of response expected from the API call. Must be a reference type.</typeparam>
/// <remarks>
/// This class is designed to work with the Fluxor state management library and provides a structured
/// approach to handling API calls. Each implementation must specify how to serialize responses,
/// what actions to dispatch on success/failure, and what fetch action initiated the request.
/// </remarks>
public abstract class ApiLoadPlan<TResponse>
    where TResponse : class
{
    /// <summary>
    /// Gets the action that initiated this fetch operation, containing metadata about the request
    /// such as caching duration, loading states, and error handling preferences.
    /// </summary>
    public FetchStartedAction FetchStartedAction { get; init; }

    /// <summary>
    /// Gets the JSON type information required for deserializing the API response.
    /// This property must provide the appropriate <see cref="JsonTypeInfo{T}"/> for the response type
    /// to enable System.Text.Json source generation and AOT compilation support.
    /// </summary>
    public abstract JsonTypeInfo<TResponse> ResponseJsonContext { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiLoadPlan{TResponse}"/> class.
    /// </summary>
    /// <param name="fetchStartedAction">The action that initiated this API request, containing request metadata and configuration.</param>
    protected ApiLoadPlan(FetchStartedAction fetchStartedAction)
    {
        FetchStartedAction = fetchStartedAction;
    }

    /// <summary>
    /// Creates the success action to be dispatched when the API call completes successfully.
    /// This method is called by the API client after successfully deserializing the response.
    /// </summary>
    /// <param name="response">The successfully deserialized response from the API.</param>
    /// <returns>A <see cref="FetchSuccessAction"/> that will be dispatched to update the application state.</returns>
    public abstract FetchSuccessAction GetSuccessAction(TResponse response);

    /// <summary>
    /// Creates the failure action to be dispatched when the API call fails.
    /// This method is called by the API client when an error occurs during the request or response processing.
    /// </summary>
    /// <param name="apiError">The error information containing details about what went wrong during the API call.</param>
    /// <returns>A <see cref="FetchFailureAction"/> that will be dispatched to handle the error state.</returns>
    public abstract FetchFailureAction GetFailureAction(ApiError apiError);
}

/// <summary>
/// Abstract base class for API request plans that include a request body, extending <see cref="ApiLoadPlan{TResponse}"/>
/// to support HTTP operations like POST and PUT that require serializing request data.
/// </summary>
/// <typeparam name="TResponse">The type of response expected from the API call. Must be a reference type.</typeparam>
/// <typeparam name="TBody">The type of the request body to be serialized and sent with the API call. Must be a reference type.</typeparam>
/// <remarks>
/// This class is used for HTTP operations that require sending data in the request body, such as creating or updating resources.
/// It provides the necessary JSON serialization context for both the request body and response, enabling proper
/// serialization/deserialization with System.Text.Json source generation.
/// </remarks>
public abstract class ApiLoadPlanWithBody<TResponse, TBody> : ApiLoadPlan<TResponse>
    where TResponse : class
    where TBody : class
{
    /// <summary>
    /// Gets the JSON type information required for serializing the request body.
    /// This property must provide the appropriate <see cref="JsonTypeInfo{T}"/> for the body type
    /// to enable System.Text.Json source generation and AOT compilation support.
    /// </summary>
    public abstract JsonTypeInfo<TBody> BodyJsonContext { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiLoadPlanWithBody{TResponse, TBody}"/> class.
    /// </summary>
    /// <param name="fetchStartedAction">The action that initiated this API request, containing request metadata and configuration.</param>
    protected ApiLoadPlanWithBody(FetchStartedAction fetchStartedAction) : base(fetchStartedAction) { }
}

/// <summary>
/// Abstract base class for API request plans that send a request body but expect no meaningful response content,
/// typically used for operations that return HTTP 201 Created or 204 No Content status codes.
/// </summary>
/// <typeparam name="TBody">The type of the request body to be serialized and sent with the API call. Must be a reference type.</typeparam>
/// <remarks>
/// This class is commonly used for POST and PUT operations where the server performs an action (like creating or updating a resource)
/// but doesn't return any data in the response body. The response type is fixed as <see cref="NoContentResponse"/>.
/// </remarks>
public abstract class ApiLoadPlanWithBodyNoContent<TBody> : ApiLoadPlanWithBody<NoContentResponse, TBody>
    where TBody : class
{
    /// <summary>
    /// Gets the JSON type information for the response, which is not applicable for no-content responses.
    /// This property is sealed to prevent overriding and throws <see cref="NotImplementedException"/> 
    /// because no-content responses do not require JSON deserialization.
    /// </summary>
    /// <exception cref="NotImplementedException">Always thrown since no-content responses don't require JSON context.</exception>
    public sealed override JsonTypeInfo<NoContentResponse> ResponseJsonContext =>
        throw new NotImplementedException("No content response does not require a JSON context.");

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiLoadPlanWithBodyNoContent{TBody}"/> class.
    /// </summary>
    /// <param name="fetchStartedAction">The action that initiated this API request, containing request metadata and configuration.</param>
    protected ApiLoadPlanWithBodyNoContent(FetchStartedAction fetchStartedAction) : base(fetchStartedAction) { }
}

