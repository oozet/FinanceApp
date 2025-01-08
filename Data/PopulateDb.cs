using FinanceApp.Controllers;
using FinanceApp.Models;
using FinanceApp.Repositories;

namespace FinanceApp.Data;

public class PopulateDb
{
    private readonly UserController _userController;
    private readonly AccountController _accountController;
    private readonly TransactionRepository _transactionRepository;

    public PopulateDb() // Parameterless constructor for model binding (won't be used)
    { }

    public PopulateDb(
        UserController userController,
        AccountController accountController,
        TransactionRepository transactionRepository
    )
    {
        _userController = userController;
        _accountController = accountController;
        _transactionRepository = transactionRepository;
    }

    public async Task Populate(int count)
    {
        AppUser user = await _userController.SignInAsync("admin", "password");

        if (user == null)
        {
            user = await _userController.CreateUser("admin", "password");
        }

        Account account = await _accountController.CreateDebugAccountAsync(
            user,
            AccountType.Business
        );
        long accountNumber = account.AccountNumber;

        var transactions = new List<TransactionData>();
        Random random = new Random();
        long totalAmount = 0;

        DateTime startDate = new DateTime(2023, 1, 1);
        TimeSpan timeSpan = DateTime.Now - startDate;

        for (int i = 0; i < count; i++)
        {
            long amountMinorUnit = (long)random.Next(1000, 10000000);
            TimeSpan randomSpan = new TimeSpan(0, random.Next(0, (int)timeSpan.TotalMinutes), 0);
            DateTime randomDate = startDate + randomSpan;

            random.Next(0, 1);
            TransactionType transactionType = (TransactionType)random.Next(0, 1);

            if (totalAmount - amountMinorUnit < 0)
            {
                transactionType = TransactionType.Deposit;
            }

            TransactionData entry =
                new()
                {
                    Id = Guid.NewGuid(),
                    AmountMinorUnit = (long)random.Next(1000, 10000000),
                    AccountNumber = accountNumber,
                    TransactionType = transactionType,
                    CreatedAt = randomDate,
                };

            // Delete every 95th transaction.
            if (i % 95 == 1)
            {
                TimeSpan spanFromDate = DateTime.Now - entry.CreatedAt;
                randomSpan = new TimeSpan(0, random.Next(0, (int)spanFromDate.TotalMinutes), 0);
                randomDate = entry.CreatedAt + randomSpan;
                entry.DeletedAt = randomDate;
            }

            totalAmount += entry.AmountMinorUnit;
            await _transactionRepository.AddAsync(entry);
        }
    }
}
