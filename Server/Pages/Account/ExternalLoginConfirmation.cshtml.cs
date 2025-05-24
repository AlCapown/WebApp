using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Common.Extensions;
using WebApp.Database.Tables;
using WebApp.Server.Services.AccountService.Command;
using WebApp.Server.Services.InviteCodeService.Query;

namespace WebApp.Server.Pages.Account;

[ValidateAntiForgeryToken]
public class ExternalLoginConfirmationModel : PageModel
{
    [Required]
    [MaxLength(50)]
    [Display(Name = "First Name")]
    [BindProperty]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    [Display(Name = "Last Name")]
    [BindProperty]
    public string LastName { get; set; }

    [Required]
    [MaxLength(30)]
    [Display(Name = "Invite Code")]
    [BindProperty]
    public string InviteCode { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; }


    private readonly SignInManager<AppUser> _signInManager;
    private readonly IMediator _mediator;

    public ExternalLoginConfirmationModel(SignInManager<AppUser> signInManager, IMediator mediator)
    {
        _signInManager = signInManager;
        _mediator = mediator;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            return RedirectToPage("Account/Login");
        }

        FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)?.ToTitleCase();
        LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)?.ToTitleCase();
        Email = info.Principal.FindFirstValue(ClaimTypes.Email);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            return RedirectToPage("/Account/Login");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var inviteCodeValidation = await _mediator.Send(new InviteCodeIsValid.Query
        {
            InviteCode = InviteCode
        }, HttpContext.RequestAborted);

        if (!inviteCodeValidation.IsValid) 
        {
            ModelState.AddModelError(nameof(InviteCode), "Invite code is not valid");
            return Page();
        }

        try
        {
            var appUser = await _mediator.Send(new CreateAppUser.Command
            {
                ExternalLoginInfo = info,
                FirstName = FirstName,
                LastName = LastName
            }, HttpContext.RequestAborted);

            // TODO: Actually check if the user was created successfully
            await _signInManager.SignInAsync(appUser.AsT0, isPersistent: true);
            return Redirect(Url.Content("~/"));
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
        }
        
        return Page();
    }
}
