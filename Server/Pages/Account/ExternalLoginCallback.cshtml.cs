using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebApp.Database.Tables;

namespace WebApp.Server.Pages.Account;

[AllowAnonymous]
[ValidateAntiForgeryToken]
public class ExternalLoginCallbackModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<ExternalLoginCallbackModel> _logger;

    public ExternalLoginCallbackModel(SignInManager<AppUser> signInManager, ILogger<ExternalLoginCallbackModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            _logger.LogWarning("ExternalLoginInfo was null on callback");
            return RedirectToPage("/Account/Login");
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);

        // Login Successful.
        if (signInResult.Succeeded)
        {
            return Redirect(Url.Content("~/"));
        }

        // User is locked or not allowed to login.
        if (signInResult.IsLockedOut || signInResult.IsNotAllowed)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        // First time the user is logging in. Redirect to external login confirmation page to confirm some information
        // before creating their account. 
        return RedirectToPage("/Account/ExternalLoginConfirmation");
    }
}
