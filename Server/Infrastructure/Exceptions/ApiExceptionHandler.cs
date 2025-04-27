using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using WebApp.Common.Models;

namespace WebApp.Server.Infrastructure.Exceptions;

public static class ApiExceptionHandler
{
    public static ExceptionHandlerOptions Get()
    {
        return new ExceptionHandlerOptions
        {
            ExceptionHandler = async (context) =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                ApiError errorResponse = null;

                if (feature == null)
                {
                    errorResponse = GetUnknownErrorResponse(context.TraceIdentifier);
                }
                else
                {
                    errorResponse = GetErrorResponse(context.TraceIdentifier, feature.Error);
                }

                context.Response.StatusCode = errorResponse.StatusCode;
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        };
    }

    private static ApiError GetErrorResponse(string traceId, Exception ex)
    {
        if (ex is WebAppValidationException webAppValidationException)
        {
            return new ApiError
            {
                StatusCode = 400,
                Message = "The following fields are invalid.",
                Errors = webAppValidationException.GetErrors(),
                TraceId = traceId
            };
        }

        return new ApiError
        {
            StatusCode = 500,
            Message = ex.Message,
            TraceId = traceId
        };
    }

    private static ApiError GetUnknownErrorResponse(string traceId)
    {
        return new ApiError
        {
            StatusCode = 500,
            Message = "An unknown error occurred.",
            TraceId = traceId
        };
    }
}
