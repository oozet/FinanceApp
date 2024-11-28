using System.Collections;

// TransactionManager works with the internal List of all transactions.
public class ListTransactionManager : ITransactionManager
{
    protected List<TransactionEntry> transactions = new List<TransactionEntry>();
    public int UidCounter { get; set; } = 0;

    public virtual TransactionEntry CreateTransaction(float amount, TransactionType type)
    {
        TransactionEntry entry = new TransactionEntry
        {
            Uid = Guid.NewGuid(),
            Date = DateTime.Now,
            Type = type,
            AmountInMinorUnit = (long)(amount * 100),
        };

        return entry;
    }

    public virtual void AddTransaction(TransactionEntry transaction)
    {
        UidCounter++;
        transactions.Add(transaction);
        Console.WriteLine(transaction.ToString());
    }

    public virtual void RemoveEntry(Guid uid)
    {
        TransactionEntry? entry = transactions.Find(x => x.Uid == uid);
        if (entry == null)
        {
            throw new NullReferenceException("That entry does not exist.");
        }
        else if (entry.DeletedAt != null)
        {
            throw new Exception($"That entry has already been removed {entry.DeletedAt}");
        }

        DateTime DeletedAt = DateTime.Now;
        entry.DeletedAt = DeletedAt;
        Console.WriteLine(entry.ToString());
    }

    public virtual void RemoveTransaction(Guid uid)
    {
        TransactionEntry? entry = transactions.Find(x => x.Uid == uid);
        transactions.Remove(entry);
    }

    public virtual List<TransactionEntry> Populate(int count)
    {
        List<TransactionEntry> newTransactions = new();

        Random random = new Random();
        float totalAmount = 0;

        DateTime startDate = new DateTime(1999, 1, 1);
        TimeSpan timeSpan = DateTime.Now - startDate;

        for (int i = 0; i < count; i++)
        {
            TimeSpan randomSpan = new TimeSpan(0, random.Next(0, (int)timeSpan.TotalMinutes), 0);
            DateTime randomDate = startDate + randomSpan;
            TransactionType transactionType = 0;
            if (totalAmount < 30000)
            {
                transactionType = (TransactionType)random.Next(0, 1);
            }

            TransactionEntry entry =
                new()
                {
                    Uid = Guid.NewGuid(),
                    Type = transactionType,
                    AmountInMinorUnit = random.Next(100, 1000000),
                    Date = randomDate,
                };

            // Get some larger amounts.
            if (i % 13 == 0)
            {
                entry.AmountInMinorUnit = random.Next(100000, 10000000);
            }
            // Get some decimal values.
            if (i % 20 == 0)
            {
                entry.AmountInMinorUnit += random.Next(0, 100);
            }

            // Mark every 95th entry as Deleted.
            if (i % 95 == 1)
            {
                TimeSpan spanFromDate = DateTime.Now - entry.Date;
                randomSpan = new TimeSpan(0, random.Next(0, (int)spanFromDate.TotalMinutes), 0);
                randomDate = entry.Date + randomSpan;
                entry.DeletedAt = randomDate;
            }

            Console.WriteLine($"Added {count} entries to the list of transactions.");
            totalAmount += entry.Amount;
            newTransactions.Add(entry);
        }

        transactions.AddRange(newTransactions);

        return newTransactions;
    }

    public float GetTotal()
    {
        float total = 0;
        foreach (TransactionEntry entry in transactions)
        {
            if (entry.Type == TransactionType.Deposit)
            {
                total += entry.Amount;
            }
            else
            {
                total -= entry.Amount;
            }
        }
        return total;
    }

    public (float, float) GetTotalPerType(List<TransactionEntry> filteredTransactions)
    {
        float deposit = 0;
        float withdrawal = 0;
        foreach (TransactionEntry entry in filteredTransactions)
        {
            if (entry.Type == TransactionType.Deposit)
            {
                deposit += entry.Amount;
            }
            else
            {
                withdrawal += entry.Amount;
            }
        }
        return (deposit, withdrawal);
    }

    public List<TransactionEntry> FilterTransactions(Predicate<TransactionEntry> predicate)
    {
        return transactions.FindAll(predicate);
    }

    public List<TransactionEntry> GetAllTransactions()
    {
        return transactions;
    }

    public TransactionEntry? GetTransaction(Guid uid)
    {
        foreach (TransactionEntry entry in transactions)
        {
            if (entry.Uid.Equals(uid))
            {
                return entry;
            }
        }
        return null;
    }
}
