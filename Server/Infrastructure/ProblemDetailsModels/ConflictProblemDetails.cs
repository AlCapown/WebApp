#nullable enable

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Server.Infrastructure.ProblemDetailsModels;

public sealed class ConflictProblemDetails : ProblemDetails
{
    public ConflictProblemDetails(string detail)
    {
        Title = "Conflict";
        Status = StatusCodes.Status409Conflict;
        Detail = detail;
    }
}
