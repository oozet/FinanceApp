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
    public string MonthYear { get; set; } = string.Empty;

    [BindProperty]
    public string Week { get; set; } = string.Empty;

    [BindProperty]
    public string Year { get; set; } = string.Empty;

    [BindProperty]
    public DateTime EndDate { get; set; }

    [BindProperty]
    public int AccountNumber { get; set; }

    [BindProperty]
    public string TransactionId { get; set; } = string.Empty;

    public List<TransactionData> Transactions { get; set; } = [];
    public List<Account> Accounts { get; set; } = [];
    public float? Withdrawn { get; set; }
    public float? Deposited { get; set; }

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
                break;

            case "month":
                Transactions = await GetMonthAsync();
                break;

            case "week":
                Transactions = await GetWeekAsync();
                break;

            case "year":
                Transactions = await GetYearAsync();
                break;

            default:
                Transactions = [];
                ErrorMessage = "Invalid form.";
                break;
        }

        Deposited = 0;
        Withdrawn = 0;
        foreach (var entry in Transactions)
        {
            if (entry.TransactionType == TransactionType.Deposit)
            {
                Deposited += entry.Amount;
            }
            else
            {
                Withdrawn += entry.Amount;
            }
        }

        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            Accounts = await _accountController.GetUserAccountsAsync(userId);
            var currentAccount = Accounts.Find(a => a.AccountNumber == AccountNumber);
            if (currentAccount != null)
            {
                TempData["Balance"] = (currentAccount.BalanceMinorUnit / 100).ToString();
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid transactionId)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            Accounts = await _accountController.GetUserAccountsAsync(userId);
        }

        var entity = await _transactionController.GetTransactionByIdAsync(transactionId);

        if (entity == null)
        {
            ErrorMessage = "Error: delete malfunction.";
            return RedirectToPage();
        }

        try
        {
            await _transactionController.DeleteTransactionAsync(entity);
        }
        catch
        {
            TempData["ErrorMessage"] = "Unable to delete transaction. Balance can not be negative.";
            return RedirectToPage();
        }

        TempData["SuccessMessage"] = $"Transaction with id {entity.Id} deleted successfully.";
        return RedirectToPage();
    }

    public async Task<List<TransactionData>> GetBetweenDatesAsync()
    {
        EndDate = EndDate.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
        return await _transactionController.GetTransactionListBetweenDatesAsync(
            StartDate,
            EndDate,
            AccountNumber
        );
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

    public async Task<List<TransactionData>> GetYearAsync()
    {
        if (int.TryParse(Year, out int parsedYear))
        {
            return await _transactionController.GetTransactionListByYearAsync(
                parsedYear,
                AccountNumber
            );
        }
        ErrorMessage = "Invalid Year format.";
        return [];
    }
}
