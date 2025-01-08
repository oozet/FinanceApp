using System.Globalization;
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

    [BindProperty]
    public string TransactionId { get; set; }

    public List<TransactionData> Transactions { get; set; }
    public List<Account> Accounts { get; set; }

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

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
                Transactions = await GetBetweenDatesAsync();
                Console.WriteLine("Got between: " + Transactions.Count);
                break;

            case "month":
                Transactions = await GetMonthAsync();
                break;

            case "week":
                Console.WriteLine("week: " + AccountNumber);
                Transactions = await GetWeekAsync();
                Console.WriteLine("Got between: " + Transactions.Count);
                break;

            default:
                Transactions = [];
                ErrorMessage = "Invalid form.";
                break;
        }

        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            Accounts = await _accountController.GetUserAccountsAsync(userId);
        }
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        Console.WriteLine("OnPostDeleteAsync reached.");
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            Accounts = await _accountController.GetUserAccountsAsync(userId);
        }

        if (!Guid.TryParse(TransactionId, out Guid id))
        {
            ErrorMessage = "id failed to parse to Guid.";
            return RedirectToPage();
        }

        var entity = await _transactionController.GetTransactionByIdAsync(id);

        if (entity == null)
        {
            ErrorMessage = "Error: delete malfunction.";
            return RedirectToPage();
        }

        await _transactionController.DeleteTransactionAsync(entity);

        TempData["SuccessMessage"] = $"Transaction with id {entity.Id} deleted successfully.";
        return RedirectToPage();
    }

    public async Task<List<TransactionData>> GetBetweenDatesAsync()
    {
        EndDate = EndDate.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
        Console.WriteLine($"{StartDate}, {EndDate}, {AccountNumber}");
        var trans = await _transactionController.GetTransactionListBetweenDatesAsync(
            StartDate,
            EndDate,
            AccountNumber
        );
        Console.WriteLine("Count: " + trans.Count);
        return trans;
    }

    public async Task<List<TransactionData>> GetMonthAsync()
    {
        if (
            DateTime.TryParseExact(
                MonthYear,
                "yyyy-MM",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedMonthYear
            )
        )
        {
            Console.WriteLine(
                $"Month year: {parsedMonthYear}, +1 month {parsedMonthYear.AddMonths(1)}, {AccountNumber}"
            );
            StartDate = parsedMonthYear;
            EndDate = parsedMonthYear
                .AddMonths(1)
                .AddDays(-1)
                .AddHours(23)
                .AddMinutes(59)
                .AddSeconds(59)
                .AddMilliseconds(999);
            ;

            return await _transactionController.GetTransactionListBetweenDatesAsync(
                StartDate,
                EndDate,
                AccountNumber
            );
        }
        ErrorMessage = "Invalid Year/Month format.";
        return [];
    }

    public async Task<List<TransactionData>> GetWeekAsync()
    {
        int year = int.Parse(Week[..4]);
        if (!int.TryParse(Week[6..], out int week))
        {
            ErrorMessage = "Invalid week format.";
            return [];
        }

        StartDate = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
        EndDate = StartDate
            .AddDays(6)
            .AddHours(23)
            .AddMinutes(59)
            .AddSeconds(59)
            .AddMilliseconds(999);
        ;

        return await _transactionController.GetTransactionListBetweenDatesAsync(
            StartDate,
            EndDate,
            AccountNumber
        );
    }
}
