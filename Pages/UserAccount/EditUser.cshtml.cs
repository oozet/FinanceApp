using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using FinanceApp.Controllers;
using FinanceApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceApp.Pages.UserAccount;

[Authorize]
public class EditUserModel : PageModel
{
    private readonly UserController _userController;
    private readonly PersonController _personController;

    public EditUserModel(UserController userController, PersonController personController)
    {
        _userController = userController;
        _personController = personController;
    }

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    [TempData]
    public string SuccessMessage { get; set; } = string.Empty;

    [BindProperty, Required]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty, Required]
    public string LastName { get; set; } = string.Empty;

    [BindProperty, Required]
    public string Email { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        AppUser? user = await _userController.GetCurrentUserAsync();
        if (user == null)
        {
            ErrorMessage = "Invalid user";
            return Page();
        }
        Person? person = await _personController.GetPersonAsync(user);
        if (person != null)
        {
            FirstName = person.FirstName;
            LastName = person.LastName;
            Email = person.Email;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine("Modelstate invalid");
            return Page();
        }

        AppUser? user = await _userController.GetCurrentUserAsync();
        if (user == null)
        {
            ErrorMessage = "Invalid user";
            return Page();
        }

        Person? person = await _personController.EditPersonAsync(user, FirstName, LastName, Email);

        if (person == null)
        {
            ErrorMessage = "Error creating person";
            return Page();
        }

        SuccessMessage = "Operation successful.";

        // If we got this far, something failed, redisplay form
        return Page();
    }
}
