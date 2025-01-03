using System.Security.Claims;
using FinanceApp.Models;
using FinanceApp.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;

namespace FinanceApp.Controllers;

public class TransactionController : Controller
{
    private readonly TransactionRepository _transactionRepository;
    private readonly UserController _userController;
    private readonly CacheService _cacheService;
    private const string KEY_PREFIX = "Transaction_";

    public TransactionController(
        TransactionRepository transactionRepository,
        UserController userController,
        CacheService cacheService
    )
    {
        _transactionRepository = transactionRepository;
        _userController = userController;
        _cacheService = cacheService;
    }

    public async Task<TransactionData?> AddTransactionAsync(
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
            CreatedAt = DateTime.Now,
        };

        try
        {
            var user = await _userController.GetCurrentUserAsync();
            if (user == null)
            {
                throw new NullReferenceException("User is null.");
            }
            try
            {
                int rowsAffected = await _transactionRepository.AddAsync(transactionEntry);
                Console.WriteLine("Rows affected: " + rowsAffected);
                if (rowsAffected == 0)
                {
                    throw new Exception("Unable to add transaction");
                }
                // string cacheKey = KEY_PREFIX + account.AccountNumber.ToString();
                // _cacheService.Set(cacheKey, account);
                // return account;
                return transactionEntry;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("" + ex.Message);
        }
        return null;
    }

    public async Task<Result> AddToQueueAsync(
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
            CreatedAt = DateTime.Now,
        };

        try
        {
            string cacheKey = KEY_PREFIX + transactionEntry.Id.ToString();
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(
                TimeSpan.FromMinutes(30)
            );
            _cacheService.Set(cacheKey, transactionEntry, cacheEntryOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Cache error: " + ex.Message);
            return Result.CacheError;
        }

        List<string> transactionKeys = [];
        foreach (var key in _cacheService.GetKeys())
        {
            if (key.Contains(KEY_PREFIX))
            {
                transactionKeys.Add(key);
            }
        }

        if (transactionKeys.Count < 5)
        {
            return Result.NoChange;
        }

        List<TransactionData> transactions = [];
        try
        {
            foreach (var key in transactionKeys)
            {
                _cacheService.TryGetValue(key, out TransactionData transaction);
                transactions.Add(transaction);
                _cacheService.Remove(key);
            }

            foreach (var transaction in transactions)
            {
                Console.WriteLine(
                    $"{transaction.Id}, {transaction.AccountNumber}, {transaction.AmountMinorUnit / 100}, {transaction.TransactionType}"
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("MemoryCache error: " + ex.Message);
            return Result.CacheError;
        }

        // Add transaction to database.
        try
        {
            var user = await _userController.GetCurrentUserAsync();
            if (user == null)
            {
                Console.WriteLine("user is null.");
                throw new Exception("No user ");
            }
            int rowsAffected = await _transactionRepository.AddTransactionEntriesAsync(
                transactions
            );
            Console.WriteLine("Rows affected: " + rowsAffected);
            if (rowsAffected == 0)
            {
                throw new Exception("Unable to add transaction");
            }
            return Result.Success;
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
            return Result.DatabaseError;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Other error: " + ex.Message);
            return Result.Error;
        }
    }

    public async Task<IEnumerable<TransactionData>> GetTransactionListAsync(long accountNumber)
    {
        try
        {
            return await _transactionRepository.GetAllFromAccountAsync(accountNumber);
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Other error: " + ex.Message);
        }
        return [];
    }

    public async Task<List<TransactionData>> GetTransactionListBetweenDatesAsync(
        DateTime startDate,
        DateTime endDate,
        long accountNumber
    )
    {
        try
        {
            return await _transactionRepository.GetTransactionsBetweenDatesAsync(
                startDate,
                endDate,
                accountNumber
            );
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Other error: " + ex.Message);
        }
        return [];
    }
}
