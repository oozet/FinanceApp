using System.Security.Claims;
using FinanceApp.Models;
using FinanceApp.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Controllers;

//[ApiController]
public class AccountController : Controller
{
    private readonly AccountRepositorySQL _accountRepository;
    private readonly UserController _userController;
    private readonly CacheService _cacheService;
    private const string KEY_PREFIX = "Account_";

    public AccountController(
        AccountRepositorySQL accountRepository,
        UserController userController,
        CacheService cacheService
    )
    {
        _accountRepository = accountRepository;
        _userController = userController;
        _cacheService = cacheService;
    }

    public async Task<Account?> CreateAccountAsync(AccountType type)
    {
        Console.WriteLine("Trying to CreateAccount");
        var user = await _userController.GetCurrentUserAsync();
        Console.WriteLine("Got user.");
        if (user == null)
        {
            Console.WriteLine("user is null.");
            throw new Exception("No user ");
        }
        Console.WriteLine("Trying to create account.");
        Account account = await _accountRepository.CreateAccountAsync(type, user.Id);
        string cacheKey = KEY_PREFIX + account.AccountNumber.ToString();
        _cacheService.Set(cacheKey, account);
        return account;
    }

    public async Task<List<Account>> GetUserAccountsAsync(string userId)
    {
        return await _accountRepository.GetAllAccountsAsync(userId);
    }

    public async Task<float> GetBalance(long accountNumber)
    {
        string cacheKey = KEY_PREFIX + accountNumber.ToString();
        if (_cacheService.TryGetValue(cacheKey, out Account? account))
        {
            return account!.BalanceMinorUnit / 100;
        }
        account = await _accountRepository.GetAsync(accountNumber);

        if (account == null)
        {
            throw new Exception("Account not found.");
        }

        return account.BalanceMinorUnit / 100;
    }
}
