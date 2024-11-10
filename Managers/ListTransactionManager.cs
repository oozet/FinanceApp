using System.Collections;

// TransactionManager works with the internal List of all transactions.
public class TransactionManager : ITransactionManager
{
    protected List<TransactionEntry> transactions = new List<TransactionEntry>();
    public int UidCounter { get; set; } = 0;

    // Virtual methods to be able to add functionality to the methods in a derived class.
    // ------------------------------------------

    // Sets the UidCounter to the highest Uid +1.
    public virtual void OnProgramLoad()
    {
        UidCounter = 0;
        foreach (TransactionEntry entry in transactions)
        {
            if (entry.Uid >= UidCounter)
                UidCounter = entry.Uid + 1;
        }
    }

    public virtual void AddEntry(float amount, TransactionType type)
    {
        TransactionEntry entry = new TransactionEntry
        {
            Uid = UidCounter,
            Date = DateTime.Now,
            Type = type,
            Amount = amount,
        };

        AddTransaction(entry);
    }

    // Added step for testing to work.
    public virtual void AddTransaction(TransactionEntry transaction)
    {
        UidCounter++;
        transactions.Add(transaction);
        Console.WriteLine(transaction.ToString());
    }

    public virtual void RemoveEntry(int uid)
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

    public virtual void RemoveTransaction(int uid)
    {
        TransactionEntry? entry = transactions.Find(x => x.Uid == uid);
        transactions.Remove(entry);
    }

    public virtual void Populate(int count)
    {
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
                    Uid = UidCounter,
                    Type = transactionType,
                    Amount = (float)random.Next(1, 10000),
                    Date = randomDate,
                };

            // Get some larger amounts.
            if (i % 13 == 0)
            {
                entry.Amount = (float)random.Next(1000, 100000);
            }
            // Get some decimal values.
            if (i % 20 == 0)
            {
                entry.Amount += (float)random.Next(0, 100) / 100;
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
            transactions.Add(entry);
            UidCounter++;
        }
    }

    // These methods only work with List directly and will never be used to save data
    // so they will never be overridden.
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

    public TransactionEntry? GetTransaction(int Uid)
    {
        foreach (TransactionEntry entry in transactions)
        {
            if (entry.Uid.Equals(Uid))
            {
                return entry;
            }
        }
        return null;
    }
}
