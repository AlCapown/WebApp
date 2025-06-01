using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Common.Extensions;
using WebApp.Database.Tables;
using WebApp.Server.Features.Account;
using WebApp.Server.Features.InviteCode;
using WebApp.Server.Infrastructure;

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
    private readonly ILogger<ExternalLoginConfirmationModel> _logger;

    public ExternalLoginConfirmationModel(
        SignInManager<AppUser> signInManager, 
        IMediator mediator, 
        ILogger<ExternalLoginConfirmationModel> logger)
    {
        _signInManager = signInManager;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info is null)
        {
            _logger.LogInformation("External login info was NULL. Redirecting to login.");
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

        if (info is null)
        {
            _logger.LogInformation("External login info was NULL. Redirecting to login.");
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
            ModelState.AddModelError(nameof(InviteCode), "Invite code is not valid.");
            return Page();
        }

        try
        {
            var appUserResult = await _mediator.Send(new CreateUser.Command
            {
                ExternalLoginInfo = info,
                FirstName = FirstName,
                LastName = LastName
            }, HttpContext.RequestAborted);

            if (appUserResult.TryPickT1(out InternalServerErrorProblemDetails problem, out AppUser appUser))
            {
                ModelState.AddModelError(string.Empty, problem.Detail);
                return Page();
            }

            await _signInManager.SignInAsync(appUser, isPersistent: true);
            return Redirect(Url.Content("~/"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user.");
            ModelState.AddModelError(string.Empty, "Failed to create user.");
        }
        
        return Page();
    }
}
