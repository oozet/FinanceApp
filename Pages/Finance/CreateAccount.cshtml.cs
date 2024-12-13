using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using FinanceApp.Controllers;
using FinanceApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceApp.Pages.Finance;

[Authorize]
public class CreateAccountModel : PageModel
{
    private readonly AccountController _accountController;

    public CreateAccountModel(AccountController accountController)
    {
        _accountController = accountController;
    }

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Transaction type is required.")]
    public AccountType AccountType { get; set; } = AccountType.Personal;

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

        if (!ModelState.IsValid)
        {
            // If we got this far, something failed, redisplay form
            return Page();
        }

        try
        {
            var account = await _accountController.CreateAccount(AccountType); // TODO: Verify username and password
            return Redirect(returnUrl);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ModelState.AddModelError(string.Empty, "Unable to create account.");

        // If we got this far, something failed, redisplay form
        return Page();
    }
}
