using System.Transactions;
using FinanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceApp.Pages;

public class TransactionModel : PageModel
{
    [BindProperty]
    protected float Amount { get; set; } = 0;

    [BindProperty]
    public TransactionType Type { get; set; }

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
