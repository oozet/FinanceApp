using System;

public abstract class TransactionBase
{
    public DateTime Date { get; init; }
    public TransactionType Type { get; init; }
    public long AmountInMinorUnit { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; set; } = null;
    public float Amount
    {
        get { return AmountInMinorUnit / 10f; }
    }

    public override string ToString()
    {
        string displayString =
            $"Type: {Type}, Amount: {Amount}\n"
            + $"{(DeletedAt.HasValue ? "Deleted at: " + DeletedAt + "\n" : String.Empty)}"
            + "-----------------------------------";
        return displayString;
    }

    public void Display()
    {
        // Don't show deleted transactions to user.
        if (DeletedAt != null)
        {
            return;
        }

        Console.WriteLine(ToString());
    }
}

public class TransactionEntry : TransactionBase
{
    public int Uid { get; set; }

    public override string ToString()
    {
        string baseString = base.ToString();
        string displayString = $"Uid: {Uid}, Date: {Date}\n" + baseString;
        return displayString;
    }
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
}
