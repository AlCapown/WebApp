using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Controllers;


[IgnoreAntiforgeryToken]
public sealed class OidcConfigurationController : ControllerBase
{

    [HttpGet(".well-known/microsoft-identity-association.json")]
    [Produces("application/json")]
    [AllowAnonymous]
    public IActionResult GetMicrosoftPublisherDomain()
    {
        return Ok(MicrosoftPublisherDomainModel.Value);
    }
}
