#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;

namespace WebApp.Client.Api;

/// <summary>
/// Represents a generic API response wrapper that encapsulates the result of an API call.
/// </summary>
/// <typeparam name="TResponse">The type of the response data when the API call is successful.</typeparam>
public sealed class ApiResponse<TResponse>
{
    /// <summary>
    /// Gets a value indicating whether the API call was successful.
    /// When true, the Response property is guaranteed to be non-null.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Response))]
    public bool IsSuccess { get; private init; }

    /// <summary>
    /// Gets the HTTP status code returned by the API.
    /// </summary>
    public int StatusCode { get; private init; }

    /// <summary>
    /// Gets the response data from the API call.
    /// This will be null when IsSuccess is false, and non-null when IsSuccess is true.
    /// </summary>
    public TResponse? Response { get; private init; }

    /// <summary>
    /// Initializes a new instance of the ApiResponse class.
    /// </summary>
    /// <param name="isSuccess">Whether the API call was successful.</param>
    /// <param name="statusCode">The HTTP status code from the API response.</param>
    /// <param name="response">The response data, must be non-null if isSuccess is true.</param>
    /// <exception cref="ArgumentException">Thrown when isSuccess is true but response is null.</exception>
    private ApiResponse(bool isSuccess, int statusCode, TResponse? response)
    {
        if (isSuccess && response is null)
        {
            throw new ArgumentException("Response cannot be null when IsSuccess is true", nameof(response));
        }

        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Response = response;
    }

    /// <summary>
    /// Creates a successful API response with the provided status code and response data.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="response">The response data returned by the API.</param>
    /// <returns>A new ApiResponse instance representing a successful API call.</returns>
    /// <exception cref="ArgumentNullException">Thrown when response is null.</exception>
    public static ApiResponse<TResponse> Success(int statusCode, TResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);
        return new ApiResponse<TResponse>(true, statusCode, response);
    }

    /// <summary>
    /// Creates a failed API response with the provided status code and no response data.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>A new ApiResponse instance representing a failed API call.</returns>
    public static ApiResponse<TResponse> Failure(int statusCode)
    {
        return new ApiResponse<TResponse>(false, statusCode, default);
    }
}
