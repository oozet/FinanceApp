using System;
using System.Collections.Generic;

namespace FinanceApp.Models;

public partial class Account
{
    public int AccountNumber { get; set; }

    public Guid? UserId { get; set; }

    public long BalanceMinorUnit { get; set; }

    public AccountType AccountType { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public float Balance
    {
        get { return BalanceMinorUnit / 100; }
    }

    // Not using ORM and lazy loading
    // public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    // public virtual User? User { get; set; }
}

public enum AccountType
{
    Personal,
    Savings,
    Business,
}
