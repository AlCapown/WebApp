#nullable enable

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Server.Infrastructure.ProblemDetailsModels;

public sealed class ForbiddenProblemDetails : ProblemDetails
{
    public ForbiddenProblemDetails(string detail)
    {
        Title = "Forbidden";
        Status = StatusCodes.Status403Forbidden;
        Detail = detail;
    }
}
