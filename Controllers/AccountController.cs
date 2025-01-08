using System.Security.Claims;
using FinanceApp.Models;
using FinanceApp.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FinanceApp.Controllers;

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
        try
        {
            var user = await _userController.GetCurrentUserAsync();

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
        catch (NpgsqlException ex)
        {
            Console.WriteLine("Error creating account in database" + ex.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected error during account creating" + ex.ToString());
        }
        return null;
    }

    public async Task<List<Account>> GetUserAccountsAsync(string userId)
    {
        try
        {
            return await _accountRepository.GetAllAccountsAsync(userId);
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine(
                $"Database error getting all accounts of user:  {userId}" + ex.ToString()
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Unexpected error trying to get all accounts from user: {userId}" + ex.ToString()
            );
        }
        return [];
    }

    public async Task<float> GetBalance(long accountNumber)
    {
        try
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
        catch (NpgsqlException ex)
        {
            Console.WriteLine(
                $"Database error getting balance of account:  {accountNumber}" + ex.ToString()
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Unexpected error getting balance of account:  {accountNumber}" + ex.ToString()
            );
        }
        return 0;
    }

    public async Task<Account?> CreateDebugAccountAsync(AppUser user, AccountType type)
    {
        try
        {
            if (user == null)
            {
                Console.WriteLine("user is null.");
                throw new Exception("No user ");
            }

            Account account = await _accountRepository.CreateAccountAsync(type, user.Id);
            string cacheKey = KEY_PREFIX + account.AccountNumber.ToString();
            _cacheService.Set(cacheKey, account);
            return account;
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("Database error creating account for debugging: " + ex.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected errorcreating account for debugging: " + ex.ToString());
        }
        return null;
    }
}
