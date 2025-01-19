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
public class DeleteUserModel : PageModel
{
    private readonly UserController _userController;

    public DeleteUserModel(UserController userController)
    {
        _userController = userController;
    }

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    [TempData]
    public string SuccessMessage { get; set; } = string.Empty;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        await _userController.DeleteCurrentUser();

        await HttpContext.SignOutAsync();
        TempData["SuccessMessage"] = "User deleted";
        return RedirectToPage("Index");
    }
}
