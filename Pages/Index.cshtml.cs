using System.Security.Claims;
using FinanceApp.Controllers;
using FinanceApp.Data;
using FinanceApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly PopulateDb _populateDb;
    private readonly UserController _userController;
    private readonly PersonController _personController;

    public IndexModel(
        ILogger<IndexModel> logger,
        PopulateDb populateDb,
        UserController userController,
        PersonController personController
    )
    {
        _logger = logger;
        _populateDb = populateDb;
        _userController = userController;
        _personController = personController;
    }

    [BindProperty]
    public CombinedUserData? UserData { get; set; }

    public async Task OnGetAsync()
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            AppUser? user = await _userController.GetCurrentUserAsync();
            UserData = await _personController.GetCombinedUserDataAsync(user!);
        }
    }

    public async Task<IActionResult> OnPostPopulateAsync()
    {
        if (await _userController.GetUserByNameAsync("admin") != null)
        {
            TempData["ErrorMessage"] = "Database already populated.";
            return Page();
        }
        await _populateDb.Populate(1000);
        var user = await _userController.SignInAsync("admin", "password"); // TODO: Verify username and password

        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );
        }
        TempData["SuccessMessage"] = "Database populated with 1000 entries";
        return RedirectToPage();
    }
}
