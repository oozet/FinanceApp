using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceApp.Pages;

public class CacheTestModel : PageModel
{
    private readonly CacheService _cacheService;

    public CacheTestModel(CacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public List<string> CacheKeys { get; set; } = new List<string>();

    public void OnGet()
    {
        CacheKeys = _cacheService.GetKeys().ToList();
    }
}
