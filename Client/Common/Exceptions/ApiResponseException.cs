using System;
using WebApp.Common.Models;

namespace WebApp.Client.Common.Exceptions;

public sealed class ApiResponseException : Exception
{
    public ApiError ApiError { get; set; }

    public ApiResponseException(ApiError apiError) : base(apiError.Message)
    {
        ApiError = apiError;
    }
}
