using Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace Identity.Pages.Account.Register;

public class Index : PageModel
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly SignInManager<IdentityUser<Guid>> _signInManager;
    private readonly IStringLocalizer<Roles> _localizer;

    public Index(
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<IdentityUser<Guid>> userManager,
        SignInManager<IdentityUser<Guid>> signInManager,
        IStringLocalizer<Roles> localizer)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _signInManager = signInManager;
        _localizer = localizer;

        View = new ViewModel();
    }

    public ViewModel View { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();


    public async Task<IActionResult> OnGet(string? returnUrl)
    {
        await BuildModelAsync(returnUrl);
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            await BuildModelAsync(Input.ReturnUrl);
            return Page();
        }

        var user = new IdentityUser<Guid>
        {
            UserName = Input.Email,
            Email = Input.Email,
            EmailConfirmed = true,
        };

        var result = await _userManager.CreateAsync(user, Input.Password);
        if (result.Succeeded)
        {
            var roleExists = await _roleManager.RoleExistsAsync(Input.RoleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(Input.RoleName));
            }

            await _userManager.AddToRoleAsync(user, Input.RoleName);
            var loginResult = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, false, lockoutOnFailure: true);

            if (loginResult.Succeeded)
            {
                if (Url.IsLocalUrl(Input.ReturnUrl))
                {
                    return Redirect(Input.ReturnUrl);
                }
                if (string.IsNullOrEmpty(Input.ReturnUrl))
                {
                    return Redirect("~/");
                }

                throw new Exception("Invalid return URL");
            }
        }

        ModelState.AddModelError(string.Empty, "Ошибка регистрации пользователя.");
        await BuildModelAsync(Input.ReturnUrl);
        return Page();
    }

    private async Task BuildModelAsync(string? returnUrl)
    {
        Input = new InputModel { ReturnUrl = returnUrl ?? string.Empty };

        View = new ViewModel
        {
            RolesList = new List<SelectListItem>
            {
                new SelectListItem(_localizer[Roles.Volunteer.ToString()], Roles.Volunteer.ToString()),
                new SelectListItem(_localizer[Roles.Partner.ToString()], Roles.Partner.ToString())
            }
        };
    }
}
