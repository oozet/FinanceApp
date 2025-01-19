using FinanceApp.Controllers;
using FinanceApp.Data;
using FinanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceApp.Pages;

public class ShowAllDataModel : PageModel
{
    private readonly PersonController _personController;

    public ShowAllDataModel(PersonController personController)
    {
        _personController = personController;
    }

    [BindProperty]
    public List<CombinedUserData>? UserData { get; set; } = null;

    public async Task OnGetAsync()
    {
        UserData = await _personController.GetAllCombinedUserDataAsync();
    }
}
