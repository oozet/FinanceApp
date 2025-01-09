using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using FinanceApp.Controllers;
using FinanceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FinanceApp.Pages.Finance;

[Authorize]
public class ShowWithJoinModel : PageModel
{
    private readonly UserController _userController;
    private readonly AccountController _accountController;

    public ShowWithJoinModel(UserController userController, AccountController accountController)
    {
        _userController = userController;
        _accountController = accountController;
    }

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;

    [BindProperty]
    public long AccountNumber { get; set; }
    public List<TransactionData> Items { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        ReturnUrl = returnUrl ?? Url.Content("~/");

        AppUser? user = await _userController.GetCurrentUserAsync();
        if (user != null)
        {
            Items = await _accountController.GetTransactionsUsingJoinAsync(user);
        }
        else
        {
            ErrorMessage = "Unable to get current user.";
        }
        return Page();
    }
}
