using System;
using System.Collections.Generic;

namespace FinanceApp.Models;

public partial class AppUser
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    // public string PasswordHash { get; set; } = null!;

    // public string Salt { get; set; } = null!;

    // public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    // public virtual ICollection<Person> People { get; set; } = new List<Person>();
}
