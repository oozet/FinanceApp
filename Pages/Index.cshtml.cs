using FinanceApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly PopulateDb _populateDb;

    public IndexModel(ILogger<IndexModel> logger, PopulateDb populateDb)
    {
        _logger = logger;
        _populateDb = populateDb;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostPopulateAsync()
    {
        await _populateDb.Populate(1000);
        TempData["Success"] = "Database populated with 1000 entries";
        return Page();
    }
}
