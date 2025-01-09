using FinanceApp.Controllers;
using FinanceApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly PopulateDb _populateDb;
    private readonly UserController _userController;

    public IndexModel(
        ILogger<IndexModel> logger,
        PopulateDb populateDb,
        UserController userController
    )
    {
        _logger = logger;
        _populateDb = populateDb;
        _userController = userController;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostPopulateAsync()
    {
        if (await _userController.GetUserByNameAsync("admin") != null)
        {
            TempData["ErrorMessage"] = "Database already populated.";
            return Page();
        }
        await _populateDb.Populate(1000);
        TempData["SuccessMessage"] = "Database populated with 1000 entries";
        return Page();
    }
}
