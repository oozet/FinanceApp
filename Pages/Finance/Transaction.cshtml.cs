using System.Transactions;
using FinanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceApp.Pages;

public class TransactionModel : PageModel
{
    [BindProperty]
    public required TransactionFormModel Transaction { get; set; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Process the form data here
        // For example, save it to a database or send an email

        return RedirectToPage("ThankYou");
    }
}

public class TransactionFormModel
{
    public float Amount { get; set; }
    public TransactionType Type { get; set; }
}
