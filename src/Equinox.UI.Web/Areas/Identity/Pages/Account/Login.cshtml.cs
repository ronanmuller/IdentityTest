using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetDevPack.Identity.Model;

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty]
    public LoginUser LoginUser { get; set; }

    public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public void OnGet()
    {
        // Esse método é chamado quando a página é carregada pela primeira vez
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(LoginUser.Email, LoginUser.Password, false, true);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(LoginUser.Email);

            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("RegisterConfirmation", new { email = LoginUser.Email });
            }
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "This user is temporarily blocked");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Incorrect user or password");
        }

        return Page();
    }
}