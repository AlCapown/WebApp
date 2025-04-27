using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Database.Tables;

namespace WebApp.Server.Pages;

[AllowAnonymous]
[IgnoreAntiforgeryToken]
public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;

    public LoginModel(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public IEnumerable<AuthenticationScheme> ExternalLogins { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.Identity.IsAuthenticated)
        {
            return Redirect(Url.Content("~/"));
        }

        await SetDefaultModelState();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string provider)
    {
        if (ModelState.IsValid)
        {
            string redirectUrl = Url.Page("ExternalLoginCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        await SetDefaultModelState();
        return Page();
    }

    private async Task SetDefaultModelState()
    {
        ExternalLogins = await _signInManager.GetExternalAuthenticationSchemesAsync();
    }
}
