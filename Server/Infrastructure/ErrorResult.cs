using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp.Common.Models;

namespace WebApp.Server.Infrastructure;

public sealed class ErrorResult : IActionResult
{
    public ApiError Error { get; set; }

    public ErrorResult(ApiError error)
    {
        Error = error;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var objectResult = new ObjectResult(Error)
        {
            StatusCode = Error.StatusCode
        };

        await objectResult.ExecuteResultAsync(context);
    }
}
