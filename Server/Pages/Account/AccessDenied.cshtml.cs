using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Server.Pages.Account;

[AllowAnonymous]
[IgnoreAntiforgeryToken]
public class AccessDeniedModel : PageModel
{
    public IActionResult OnGet()
    {
        return Page();
    }

    public IActionResult OnPost()
    {
        return Redirect(Url.Content("~/"));
    }
}
