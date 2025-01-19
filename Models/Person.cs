using System;
using System.Collections.Generic;

namespace FinanceApp.Models;

public partial class Person
{
    public int PersonId { get; set; }

    public Guid UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = null!;

    //public virtual User User { get; set; } = null!;
}
