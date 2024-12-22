using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using FinanceApp.Controllers;
using FinanceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceApp.Pages.Finance;

[Authorize]
public class TransactionEntryModel : PageModel
{
    private readonly TransactionController _transactionController;
    private readonly AccountController _accountController;

    public List<Account> Accounts { get; private set; } = new List<Account>();

    public TransactionEntryModel(
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
            Accounts = await _accountController.GetUserAccounts(userId);
            TempData["Accounts"] = JsonSerializer.Serialize(Accounts);
        }
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        // Fixes List<Accounts> set to null on page reset.
        string? accountsJson = TempData["Accounts"] as string;
        Accounts = !string.IsNullOrEmpty(accountsJson)
            ? JsonSerializer.Deserialize<List<Account>>(accountsJson) ?? new List<Account>()
            : new List<Account>();

        returnUrl = returnUrl ?? Url.Content("~/");
        if (!ModelState.IsValid)
        {
            return Page();
        }

        long AmountMinorUnit = (long)(Amount * 100);

        var transactionEntry = await _transactionController.AddTransaction(
            AmountMinorUnit,
            AccountNumber,
            TransactionType
        );
        Console.WriteLine("Id: " + transactionEntry.Id);
        Console.WriteLine("Amount: " + transactionEntry.AmountMinorUnit);
        // TODO: Verify username and password

        // if (transaction != null)
        // {

        //     return Redirect(returnUrl);
        // }

        // Process the form data here
        // For example, save it to a database or send an email
        Amount = 0;
        TempData.Keep("Accounts");
        TempData["SuccessMessage"] = "Transaction completed successfully.";
        return RedirectToPage();
    }
}

// public class TransactionFormModel
// {
//     public float Amount { get; set; }
//     public TransactionType Type { get; set; }
// }
