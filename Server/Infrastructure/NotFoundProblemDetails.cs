#nullable enable

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Server.Infrastructure;

public sealed class NotFoundProblemDetails : ProblemDetails
{
    public NotFoundProblemDetails(string detail)
    {
        Title = "NotFound";
        Status = StatusCodes.Status404NotFound;
        Detail = detail;
    }
}
