using System.ComponentModel.DataAnnotations;
using System.Transactions;
using FinanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceApp.Pages;

public class TransactionModel : PageModel
{
    // [BindProperty]
    // public required TransactionFormModel Transaction { get; set; }
    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;

    [BindProperty, Required]
    public float Amount { get; set; }

    [BindProperty]
    public TransactionType Type { get; set; }

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

        return RedirectToPage("ThankYou");
    }
}

// public class TransactionFormModel
// {
//     public float Amount { get; set; }
//     public TransactionType Type { get; set; }
// }
