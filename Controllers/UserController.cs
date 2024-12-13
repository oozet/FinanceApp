using System.Security.Claims;
using FinanceApp.Models;
using FinanceApp.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

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

    public async Task<AppUser?> GetCurrentUser()
    {
        var currentUser = _httpContextAccessor.HttpContext.User;

        if (currentUser == null)
        {
            Console.WriteLine("User not authenticated."); // Handle unauthenticated user
            return null;
        }

        var userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            Console.WriteLine("Critical. User Id is missing.");
            return null;
        }

        Console.WriteLine("GetUserFromCache");
        return await GetUserFromCache(userId);
    }

    public async Task<AppUser> GetUserFromCache(string userId)
    {
        string cacheKey = KEY_PREFIX + userId;

        Console.WriteLine(userId);
        if (!_memoryCache.TryGetValue(cacheKey, out AppUser? cachedUser))
        {
            // If not found in cache, fetch from database or UserManager
            try
            {
                cachedUser = await _userRepository.GetByIdAsync(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            if (cachedUser == null)
            {
                throw new Exception("User does not exist in database.");
            }
            StoreInCache(cacheKey, cachedUser);
        }

        Console.WriteLine("User got");
        return cachedUser;
    }

    //Is this ever used?
    public void StoreUserInCache(AppUser user)
    {
        Console.WriteLine("Debug: StoreUserInCache is used.");
        string cacheKey = KEY_PREFIX + user.Id.ToString();

        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(
            TimeSpan.FromMinutes(30)
        ); // Cache duration

        return; // Return appropriate response
    }

    public void StoreInCache<T>(string cacheKey, T entity)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(
            TimeSpan.FromMinutes(30)
        ); // Cache duration

        _memoryCache.Set(cacheKey, entity, cacheEntryOptions);
        if (_memoryCache.TryGetValue(cacheKey, out AppUser? cachedUser))
        {
            Console.WriteLine("Debug: Retrieved from cache:" + cachedUser);
            Console.WriteLine(
                string.Join(
                    " ",
                    cachedUser.GetType().GetProperties().Select(prop => prop.GetValue(cachedUser))
                )
            );
        }
        else
        {
            Console.WriteLine("Debug: Retrieve from cache failed.");
        }
        return;
    }

    public async Task<AppUser?> SignIn(string username, string password)
    {
        var appUser = await _userRepository.GetUserByNameAndPasswordAsync(username, password);
        if (appUser != null)
        {
            string cacheKey = KEY_PREFIX + appUser.Id.ToString();
            StoreInCache<AppUser>(cacheKey, appUser);
        }
        return appUser;
    }

    public async Task<AppUser?> CreateUser(string username, string password)
    {
        var appUser = await _userRepository.CreateUserAsync(username, password);
        string cacheKey = KEY_PREFIX + appUser.Id.ToString();
        StoreInCache(cacheKey, appUser);
        return appUser;
    }
}
