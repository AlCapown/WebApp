#nullable enable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using WebApp.Database.Tables;

namespace WebApp.Server.Pages;

public class WasmModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public WasmModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnGet()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToPage("Account/Login");
        }

        var user = await _userManager.GetUserAsync(User);

        if (user is null || await _userManager.IsLockedOutAsync(user))
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("Account/Login");
        }

        return Page();
    }
}
