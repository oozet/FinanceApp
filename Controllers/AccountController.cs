using System.Security.Claims;
using FinanceApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Controllers;

//[ApiController]
public class AccountController : Controller
{
    private readonly IAccountRepositorySQL _accountRepository;

    public AccountController(IAccountRepositorySQL accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Account?> CreateUser(AccountType type)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if((userId != null){
            Guid id = new Guid(userId);
            return await _accountRepository.CreateAccountAsync(type, userId);
        }
        return null;
    }
}
