using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Server.Pages;

public class WasmModel : PageModel
{
    public IActionResult OnGet()
    { 
        if (HttpContext.User.Identity.IsAuthenticated)
        {
            return Page();
        }

        return RedirectToPage("Account/Login");
    }
}
