using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using FinanceApp.Controllers;
using FinanceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceApp.Pages.Finance;

[Authorize]
public class AddManyModel : PageModel
{
    private readonly TransactionController _transactionController;
    private readonly AccountController _accountController;

    public List<Account> Accounts { get; private set; } = new List<Account>();

    public AddManyModel(
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

    [BindProperty, Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, float.MaxValue, ErrorMessage = "Amount must be a positive number.")]
    public float Amount { get; set; }

    [BindProperty, Required(ErrorMessage = "Transaction type is required.")]
    public TransactionType TransactionType { get; set; }

    [BindProperty, Required(ErrorMessage = "Account number is required.")]
    public long AccountNumber { get; set; }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        ReturnUrl = returnUrl ?? Url.Content("~/");
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId != null)
        {
            Accounts = await _accountController.GetUserAccountsAsync(userId);
            TempData["Accounts"] = JsonSerializer.Serialize(Accounts);
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Fixes List<Accounts> being set to null on page reset.
        string? accountsJson = TempData["Accounts"] as string;
        Accounts = !string.IsNullOrEmpty(accountsJson)
            ? JsonSerializer.Deserialize<List<Account>>(accountsJson) ?? new List<Account>()
            : new List<Account>();

        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState Invalid!");
            return Page();
        }

        long AmountMinorUnit = (long)(Amount * 100);
        bool validTransaction = true;
        Account selectedAccount = Accounts.FirstOrDefault(e => e.AccountNumber == AccountNumber)!;

        if (TransactionType == TransactionType.Withdrawal)
        {
            if (selectedAccount.BalanceMinorUnit < AmountMinorUnit)
            {
                validTransaction = false;
            }
        }

        if (!validTransaction)
        {
            TempData["ErrorMessage"] = "Insufficient balance in account.";
        }
        else
        {
            var result = await _transactionController.AddToQueueAsync(
                AmountMinorUnit,
                selectedAccount.AccountNumber,
                TransactionType
            );
            if (result == Result.Success)
            {
                TempData["SuccessMessage"] = "Transactions completed successfully.";
            }
            else if (result == Result.NoChange)
            {
                TempData["SuccessMessage"] = "Transaction added to queue.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error: Transaction failed.";
            }
        }

        // Reset form and keep Accounts.
        Amount = 0;
        TempData.Keep("Accounts");
        return RedirectToPage();
    }
}

// public class TransactionFormModel
// {
//     public float Amount { get; set; }
//     public TransactionType Type { get; set; }
// }