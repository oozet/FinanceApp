using FinanceApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Controllers;

//[ApiController]
public class UserController : Controller
{
    private readonly IUserRepositorySQL _userRepository;

    User? currentUser;

    public UserController(IUserRepositorySQL userRepository)
    {
        _userRepository = userRepository;
    }

    public bool IsSignedIn()
    {
        return currentUser != null;
    }

    public async Task<User?> SignIn(string username, string password)
    {
        return await _userRepository.GetUserByNameAndPasswordAsync(username, password);
    }

    public async Task<User?> CreateUser(string username, string password)
    {
        return await _userRepository.CreateUserAsync(username, password);
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

    public User GetCurrentUser()
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
        currentUser = new User { Username = "Tried logout" }; // Add your logout logic here
        return RedirectToAction("Index", "Home");
    }
}
