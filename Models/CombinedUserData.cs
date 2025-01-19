using System;
using System.Collections.Generic;

namespace FinanceApp.Models;

public partial class CombinedUserData
{
    public string UserName { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public List<int> AccountNumbers { get; set; } = [];
}
