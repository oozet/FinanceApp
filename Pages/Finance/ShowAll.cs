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
public class ShowAllModel : PageModel
{
    private readonly TransactionController _transactionController;
    private readonly AccountController _accountController;

    public ShowAllModel(
        TransactionController transactionController,
        AccountController accountController
    )
    {
        _transactionController = transactionController;
        _accountController = accountController;
    }

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;

    [BindProperty]
    public long AccountNumber { get; set; }
    public List<TransactionData> Items { get; set; } = [];
    public List<Account> Accounts { get; private set; }

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        ReturnUrl = returnUrl ?? Url.Content("~/");
        await PopulateAccountsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateAccountsAsync();
        var list = await _transactionController.GetTransactionListAsync(AccountNumber);
        Items = list.ToList();
        foreach (var account in Items)
        {
            Console.WriteLine(account.AmountMinorUnit);
        }
        ViewData["Transactions"] = Items;

        return Page();
    }

    private async Task PopulateAccountsAsync()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            Accounts = await _accountController.GetUserAccountsAsync(userId);
        }
    }
}
