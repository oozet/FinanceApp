using System.Security.Claims;
using FinanceApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceApp.Controllers;

//[ApiController]
public class UserController : Controller
{
    private readonly IUserRepositorySQL _userRepository;
    protected IMemoryCache _memoryCache;

    public UserController(IUserRepositorySQL userRepository, IMemoryCache memoryCache)
    {
        _userRepository = userRepository;
        _memoryCache = memoryCache;
    }

    public async Task<AppUser> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return null;
        }

        return await GetUserFromCache(userId);
    }

    public async Task<AppUser> GetUserFromCache(string userId)
    {
        string cacheKey = userId;

        if (!_memoryCache.TryGetValue(cacheKey, out AppUser? cachedUser))
        {
            // If not found in cache, fetch from database or UserManager

            cachedUser = await _userRepository.GetByIdAsync(userId);

            if (cachedUser == null)
            {
                throw new Exception("User does not exist in database.");
            }
            StoreUserInCache(cachedUser);
        }

        return cachedUser;
    }

    public void StoreUserInCache(AppUser user)
    {
        string cacheKey = user.Id.ToString(); // Use user ID or another unique identifier

        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(
            TimeSpan.FromMinutes(30)
        ); // Cache duration

        _memoryCache.Set(cacheKey, user, cacheEntryOptions);

        return; // Return appropriate response
    }

    public async Task<AppUser?> SignIn(string username, string password)
    {
        return await _userRepository.GetUserByNameAndPasswordAsync(username, password);
    }

    public async Task<AppUser?> CreateUser(string username, string password)
    {
        return await _userRepository.CreateUserAsync(username, password);
    }

    public class SampleController : Controller
    {
        private readonly IMemoryCache _memoryCache;

        public SampleController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            string cacheKey = "appUser";
            if (!_memoryCache.TryGetValue(cacheKey, out string cachedData))
            {
                // Data is not in the cache, so fetch and cache it
                cachedData = "This is the cached data";
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(
                    TimeSpan.FromMinutes(5)
                ); // Cache duration

                _memoryCache.Set(cacheKey, cachedData, cacheEntryOptions);
            }

            return View(model: cachedData);
        }
    }

    // [HttpGet("Login")]
    // public IActionResult Login()
    // {
    //     return RedirectToAction("Index", "Home");
    // }

    // [HttpPost("Login")]
    // public IActionResult Login(string username, string password)
    // {
    //     return RedirectToAction("Index", "Home");
    //     // Add your login logic here
    //     if (username == "admin" && password == "password") // Example login logic
    //     {
    //         // Login successful
    //         currentUser = new User { Name = "admin" };
    //         return RedirectToAction("Index");
    //     }

    //     // Login failed
    //     ModelState.AddModelError(string.Empty, "Invalid login attempt.");
    //     return View();
    // }

    public AppUser GetCurrentUser()
    {
        if (currentUser == null)
        {
            throw new InvalidDataException("The user should be set but isn't.");
        }
        return currentUser;
    }

    [Route("User")]
    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        currentUser = new AppUser { Username = "Tried logout" }; // Add your logout logic here
        return RedirectToAction("Index", "Home");
    }
}
