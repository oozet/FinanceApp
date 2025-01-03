using System;
using System.Collections.Generic;

namespace FinanceApp.Models;

public partial class TransactionData
{
    public Guid Id { get; set; }

    public long AmountMinorUnit { get; set; }

    public long AccountNumber { get; set; }

    public TransactionType TransactionType { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    // Not using lazy loading and proxies
    //public virtual Account AccountNumberNavigation { get; set; } = null!;

    public float Amount
    {
        get { return AmountMinorUnit / 100; }
    }
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
}
