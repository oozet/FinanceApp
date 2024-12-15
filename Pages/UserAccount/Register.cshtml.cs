using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using FinanceApp.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceApp.Pages.UserAccount;

public class RegisterModel : PageModel
{
    private readonly UserController _userController;

    public RegisterModel(UserController userController)
    {
        _userController = userController;
    }

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;

    [BindProperty, Required]
    public string Username { get; set; } = string.Empty;

    [BindProperty, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public void OnGet(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            return Page();
        if (!ModelState.IsValid)
        {
            // If we got this far, something failed, redisplay form
            return Page();
        }

        var user = await _userController.CreateUser(Username, Password); // TODO: Verify username and password

        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            return Redirect(returnUrl);
        }

        ModelState.AddModelError(string.Empty, "Unable to create user. Username already exists.");

        // If we got this far, something failed, redisplay form
        return Page();
    }
}
