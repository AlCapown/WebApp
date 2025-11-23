using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Controllers;


[IgnoreAntiforgeryToken]
[AllowAnonymous]
public sealed class OidcConfigurationController : ControllerBase
{

    [HttpGet(".well-known/microsoft-identity-association.json")]
    [Produces("application/json")]
    public IActionResult GetMicrosoftPublisherDomain()
    {
        return Ok(MicrosoftPublisherDomainModel.Value);
    }
}
