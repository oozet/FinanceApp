using System;
using System.Collections.Generic;
using System.Text;

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

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Id: " + Id.ToString());
        sb.AppendLine("Amount in minor unit: " + AmountMinorUnit);
        sb.AppendLine("Account number: " + AccountNumber);
        sb.AppendLine("Transaction type: " + TransactionType.ToString());
        sb.AppendLine("Created at: " + CreatedAt.ToString());
        sb.AppendLine("Deleted at: " + DeletedAt.ToString());
        return sb.ToString();
    }
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
}
