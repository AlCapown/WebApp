using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Server.Pages.Account;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        return SignOut(new AuthenticationProperties
        {
            RedirectUri = "~/"
        }, IdentityConstants.ApplicationScheme);
    }
}
