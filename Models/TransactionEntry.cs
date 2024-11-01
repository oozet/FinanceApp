using System;

public class TransactionEntry
{
    public int Uid { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public float Amount { get; set; }
    public DateTime? DeletedAt { get; set; } = null;

    public override string ToString()
    {
        string displayString =
            $"Uid: {Uid}, Date: {Date}\n"
            + $"Type: {Type}, Amount: {Amount}\n"
            + $"{(DeletedAt.HasValue ? "Deleted at: " + DeletedAt + "\n" : String.Empty)}"
            + "-----------------------------------";
        return displayString;
    }

    public void Display()
    {
        // Don't show deleted transactions to user.
        if (DeletedAt != null)
            return;
        Console.WriteLine(ToString());
    }
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
}
