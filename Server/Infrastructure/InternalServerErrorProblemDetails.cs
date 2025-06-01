#nullable enable

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;

namespace WebApp.Server.Infrastructure;

public sealed class InternalServerErrorProblemDetails : ProblemDetails
{
    public InternalServerErrorProblemDetails(string detail)
    {
        Title = "Internal Server Error";
        Status = StatusCodes.Status500InternalServerError;
        Detail = detail;
    }

    public InternalServerErrorProblemDetails(Exception exception)
    {
        Title = "Internal Server Error";
        Status = StatusCodes.Status500InternalServerError;
        Detail = exception.Message;
#if DEBUG
        Extensions.Add("StackTrace", exception.StackTrace);
#endif
    }
}
