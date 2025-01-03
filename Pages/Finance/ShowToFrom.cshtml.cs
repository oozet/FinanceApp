using System.Security.Claims;
using FinanceApp.Controllers;
using FinanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ShowToFromModel : PageModel
{
    private readonly TransactionController _transactionController;
    private readonly AccountController _accountController;

    public ShowToFromModel(
        TransactionController transactionController,
        AccountController accountController
    )
    {
        _transactionController = transactionController;
        _accountController = accountController;
    }

    [BindProperty]
    public DateTime StartDate { get; set; }

    [BindProperty]
    public string MonthYear { get; set; }

    [BindProperty]
    public string Week { get; set; }

    [BindProperty]
    public DateTime EndDate { get; set; }

    [BindProperty]
    public long AccountNumber { get; set; }

    public List<TransactionData> Transactions { get; set; }
    public List<Account> Accounts { get; set; }

    public async Task OnGetAsync()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            Accounts = await _accountController.GetUserAccountsAsync(userId);
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var form = Request.Form["form"];
        switch (form)
        {
            case "between":
                GetBetweenDates();
                break;

            case "month":
                GetMonth();
                break;

            case "week":
                GetWeek();
                break;

            default:
                break;
        }
        if (
            DateTime.TryParseExact(
                MonthYear,
                "yyyy-MM",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedMonthYear
            )
        ) { }
        if (
            DateTime.TryParseExact(
                Week + "-1",
                "yyyy-'W'ww-e",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedWeek
            )
        ) { }
        Transactions = await _transactionController.GetTransactionListBetweenDatesAsync(
            StartDate,
            EndDate,
            AccountNumber
        );

        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            Accounts = await _accountController.GetUserAccountsAsync(userId);
        }
        return Page();
    }
}
