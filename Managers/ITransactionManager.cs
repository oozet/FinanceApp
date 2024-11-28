public interface ITransactionManager
{
    TransactionEntry CreateTransaction(float amount, TransactionType type);
    void AddTransaction(TransactionEntry transaction);
    void RemoveEntry(Guid uid);
    void RemoveTransaction(Guid uid);
    List<TransactionEntry> Populate(int count);
    float GetTotal();

    TransactionEntry? GetTransaction(Guid Uid);
    List<TransactionEntry> GetAllTransactions();
}

// public class TransactionManager : ITransactionManager
// {
//     public virtual TransactionEntry CreateTransaction(float amount, TransactionType type)
//     {
//         TransactionEntry entry = new TransactionEntry
//         {
//             Uid = Guid.NewGuid(),
//             Date = DateTime.Now,
//             Type = type,
//             AmountInMinorUnit = (long)(amount * 10),
//         };

//         return entry;
//     }

//     // Added step for testing to work.
//     public virtual void AddTransaction(TransactionEntry transaction)
//     {
//         Console.WriteLine(transaction.ToString());
//     }

//     public virtual void RemoveEntry(Guid uid)
//     {
//         DateTime DeletedAt = DateTime.Now;
//     }

//     public virtual void RemoveTransaction(int uid) { }

//     public virtual List<TransactionEntry> Populate(int count)
//     {
//         List<TransactionEntry> newTransactions = new();

//         Random random = new Random();
//         float totalAmount = 0;

//         DateTime startDate = new DateTime(1999, 1, 1);
//         TimeSpan timeSpan = DateTime.Now - startDate;

//         for (int i = 0; i < count; i++)
//         {
//             TimeSpan randomSpan = new TimeSpan(0, random.Next(0, (int)timeSpan.TotalMinutes), 0);
//             DateTime randomDate = startDate + randomSpan;
//             TransactionType transactionType = 0;
//             if (totalAmount < 30000)
//             {
//                 transactionType = (TransactionType)random.Next(0, 1);
//             }

//             TransactionEntry entry =
//                 new()
//                 {
//                     Uid = Guid.NewGuid(),
//                     Type = transactionType,
//                     AmountInMinorUnit = random.Next(100, 1000000),
//                     Date = randomDate,
//                 };

//             // Get some larger amounts.
//             if (i % 13 == 0)
//             {
//                 entry.AmountInMinorUnit = random.Next(100000, 10000000);
//             }
//             // Get some decimal values.
//             if (i % 20 == 0)
//             {
//                 entry.AmountInMinorUnit += random.Next(0, 100);
//             }

//             // Mark every 95th entry as Deleted.
//             if (i % 95 == 1)
//             {
//                 TimeSpan spanFromDate = DateTime.Now - entry.Date;
//                 randomSpan = new TimeSpan(0, random.Next(0, (int)spanFromDate.TotalMinutes), 0);
//                 randomDate = entry.Date + randomSpan;
//                 entry.DeletedAt = randomDate;
//             }

//             Console.WriteLine($"Added {count} entries to the list of transactions.");
//             totalAmount += entry.Amount;
//             newTransactions.Add(entry);
//         }

//         return newTransactions;
//     }

//     public virtual float GetTotal()
//     {
//         throw new NotImplementedException("Should be implemented in derived class.");
//     }

//     public List<TransactionEntry> GetAllTransactions()
//     {
//         throw new NotImplementedException("Should be implemented in derived class.");
//     }

//     public TransactionEntry? GetTransaction(Guid uid)
//     {
//         throw new NotImplementedException("Should be implemented in derived class.");
//     }

//     public void RemoveTransaction(Guid uid)
//     {
//         throw new NotImplementedException("Should be implemented in derived class.");
//     }
// }
