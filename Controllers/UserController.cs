using System.Security.Claims;
using FinanceApp.Models;
using FinanceApp.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;

namespace FinanceApp.Controllers;

public class UserController : Controller
{
    private readonly IUserRepositorySQL _userRepository;
    protected IMemoryCache _memoryCache;
    private IHttpContextAccessor _httpContextAccessor;
    private const string KEY_PREFIX = "User_";

    public UserController(
        IUserRepositorySQL userRepository,
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _userRepository = userRepository;
        _memoryCache = memoryCache;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AppUser?> GetCurrentUserAsync()
    {
        var currentUser = _httpContextAccessor.HttpContext!.User;

        if (currentUser == null)
        {
            Console.WriteLine("User not authenticated.");
            return null;
        }

        var userId =
            currentUser.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new NullReferenceException("Critical error: User id missing in claims token.");

        try
        {
            return await GetAppUserAsync(userId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<AppUser?> GetAppUserAsync(string userId)
    {
        try
        {
            string cacheKey = KEY_PREFIX + userId;

            if (!_memoryCache.TryGetValue(cacheKey, out AppUser? cachedUser))
            {
                // If not found in cache, fetch from database or UserManager
                try
                {
                    cachedUser = await _userRepository.GetByIdAsync(userId);
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine("Database error: " + ex.Message);
                }

                if (cachedUser == null)
                {
                    throw new NullReferenceException();
                }

                StoreInCache(cacheKey, cachedUser);
            }
            return cachedUser;
        }
        catch (NullReferenceException)
        {
            Console.WriteLine($"Could not find user {userId} in database.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to get user: " + ex.Message);
        }
        return null;
    }

    public void StoreInCache<T>(string cacheKey, T entity)
    {
        try
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(
                TimeSpan.FromMinutes(30)
            ); // Cache duration

            _memoryCache.Set(cacheKey, entity, cacheEntryOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not store in cache {cacheKey}: " + ex.Message);
        }
        return;
    }

    public async Task<AppUser?> SignInAsync(string username, string password)
    {
        try
        {
            var appUser = await _userRepository.GetUserByNameAndPasswordAsync(username, password);
            if (appUser != null)
            {
                string cacheKey = KEY_PREFIX + appUser.Id.ToString();
                StoreInCache<AppUser>(cacheKey, appUser);
            }
            return appUser;
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine($"Database error while signing in: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while signing in: {ex.Message}");
        }
        return null;
    }

    public async Task<AppUser?> CreateUser(string username, string password)
    {
        try
        {
            // Return null user if username is already in database.
            if (await _userRepository.GetIdByNameAsync(username) != Guid.Empty)
            {
                return null;
            }
            var appUser = await _userRepository.CreateUserAsync(username, password);
            if (appUser == null)
            {
                Console.WriteLine("Debug: user not created.");
                throw new InvalidOperationException("User not created.");
            }
            string cacheKey = KEY_PREFIX + appUser.Id.ToString();
            StoreInCache(cacheKey, appUser);
            return appUser;
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine($"Database error while creating user: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.GetType()} while creating user: {ex.Message}");
        }
        return null;
    }
}
