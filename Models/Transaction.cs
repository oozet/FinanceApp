using System;
using System.Collections.Generic;

namespace FinanceApp.Models;

public partial class Transaction
{
    public Guid Id { get; set; }

    public long AmountMinorUnit { get; set; }

    public int AccountNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Account AccountNumberNavigation { get; set; } = null!;
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
}
