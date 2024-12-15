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
    private readonly IAccountRepositorySQL _accountRepository;
    private readonly UserController _userController;
    private readonly CacheService _cacheService;
    private const string KEY_PREFIX = "Account_";

    public AccountController(
        IAccountRepositorySQL accountRepository,
        UserController userController,
        CacheService cacheService
    )
    {
        _accountRepository = accountRepository;
        _userController = userController;
        _cacheService = cacheService;
    }

    public async Task<Account?> CreateAccount(AccountType type)
    {
        Console.WriteLine("Trying to CreateAccount");
        var user = await _userController.GetCurrentUser();
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

    public async Task<List<Account>> GetUserAccounts(string userId)
    {
        return await _accountRepository.GetAllAccountsAsync(userId);
    }
}
