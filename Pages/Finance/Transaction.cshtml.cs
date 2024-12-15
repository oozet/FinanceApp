using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Transactions;
using FinanceApp.Controllers;
using FinanceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceApp.Pages.Finance;

[Authorize]
public class TransactionModel : PageModel
{
    private readonly TransactionController _transactionController;
    private readonly AccountController _accountController;

    public List<Account> Accounts { get; private set; }
    public long AccountNumber { get; set; }

    public TransactionModel(
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

    [BindProperty, Required]
    public float Amount { get; set; }

    [BindProperty, Required(ErrorMessage = "Transaction type is required.")]
    public TransactionType TransactionType { get; set; }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        ReturnUrl = returnUrl ?? Url.Content("~/");
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        Accounts = await _accountController.GetUserAccounts(userId);
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");
        if (!ModelState.IsValid)
        {
            return Page();
        }

        long AmountMinorUnit = (long)(Amount * 100);

        // var transaction = await _userController.AddTransaction(AmountMinorUnit, Type); // TODO: Verify username and password

        // if (transaction != null)
        // {

        //     return Redirect(returnUrl);
        // }

        // Process the form data here
        // For example, save it to a database or send an email

        return Page();
    }
}

// public class TransactionFormModel
// {
//     public float Amount { get; set; }
//     public TransactionType Type { get; set; }
// }
