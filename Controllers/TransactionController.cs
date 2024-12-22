using System.Security.Claims;
using FinanceApp.Models;
using FinanceApp.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Controllers;

public class TransactionController : Controller
{
    private readonly IAccountRepositorySQL _accountRepository;
    private readonly UserController _userController;
    private readonly CacheService _cacheService;
    private const string KEY_PREFIX = "Transaction_";

    public TransactionController(
        IAccountRepositorySQL accountRepository,
        UserController userController,
        CacheService cacheService
    )
    {
        _accountRepository = accountRepository;
        _userController = userController;
        _cacheService = cacheService;
    }

    public async Task<TransactionData> AddTransaction(
        long amountMinorUnit,
        long accountNumber,
        TransactionType type
    )
    {
        var transactionEntry = new TransactionData()
        {
            Id = Guid.NewGuid(),
            AmountMinorUnit = amountMinorUnit,
            AccountNumber = accountNumber,
            TransactionType = type,
            CreatedAt = DateTime.UtcNow,
        };

        // Add transaction to database.
        return transactionEntry;
        // Console.WriteLine("Trying to create transaction");
        // var user = await _userController.GetCurrentUser();
        // Console.WriteLine("Got user.");
        // if (user == null)
        // {
        //     Console.WriteLine("user is null.");
        //     throw new Exception("No user ");
        // }
        // Console.WriteLine("Trying to create account.");
        // Account account = await _accountRepository.CreateAccountAsync(type, user.Id);
        // string cacheKey = KEY_PREFIX + account.AccountNumber.ToString();
        // _cacheService.Set(cacheKey, account);
        // return account;
    }
}
