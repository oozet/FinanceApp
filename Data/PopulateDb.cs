using FinanceApp.Controllers;
using FinanceApp.Models;
using FinanceApp.Repositories;

namespace FinanceApp.Data;

public class PopulateDb
{
    private readonly UserController _userController;
    private readonly AccountController _accountController;
    private readonly TransactionRepository _transactionRepository;

    // public PopulateDb() // Parameterless constructor for model binding (won't be used)
    // { }

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
        var user = await _userController.SignInAsync("admin", "password");

        if (user == null)
        {
            user = await _userController.CreateUser("admin", "password");
        }

        var account = await _accountController.CreateDebugAccountAsync(user!, AccountType.Business);
        int accountNumber = account!.AccountNumber;

        var transactions = new List<TransactionData>();
        Random random = new Random();
        long totalAmount = 0;
        TransactionType transactionType;

        DateTime startDate = new DateTime(2023, 1, 1);
        TimeSpan timeSpan = DateTime.Now - startDate;

        for (int i = 0; i < count; i++)
        {
            long amountMinorUnit = (long)random.Next(1000, 10000000);
            TimeSpan randomSpan = new TimeSpan(0, random.Next(0, (int)timeSpan.TotalMinutes), 0);
            DateTime randomDate = startDate + randomSpan;

            if ((totalAmount - amountMinorUnit) < 0)
            {
                transactionType = TransactionType.Deposit;
            }
            else
            {
                transactionType = (TransactionType)random.Next(0, 2);
            }

            TransactionData entry =
                new()
                {
                    Id = Guid.NewGuid(),
                    AmountMinorUnit = amountMinorUnit,
                    AccountNumber = accountNumber,
                    TransactionType = transactionType,
                    CreatedAt = randomDate,
                };

            if (transactionType == TransactionType.Withdrawal)
                totalAmount -= amountMinorUnit;
            else
            {
                totalAmount += amountMinorUnit;
            }

            transactions.Add(entry);
        }

        await _transactionRepository.AddTransactionEntriesAsync(transactions);
    }
}
