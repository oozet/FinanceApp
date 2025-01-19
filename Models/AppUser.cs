using System;
using System.Collections.Generic;

namespace FinanceApp.Models;

public partial class AppUser
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    // Password and salt is hidden in object even though I guess there isn't really a problem with including them.
    // public string PasswordHash { get; set; } = null!;
    // public string Salt { get; set; } = null!;

    // Not using ORM and lazy loading
    // public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
