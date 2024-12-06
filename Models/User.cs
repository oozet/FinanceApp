using System;
using System.Collections.Generic;

namespace FinanceApp.Models;

public partial class User
{
    public Guid Id { get; set; }

    public required string Username { get; set; }
}
