public class TransactionDisplayService
{
    public void ListTransactions(
        int entriesBeforeBreak,
        List<TransactionEntry> transactions,
        bool IsAdmin
    )
    {
        if (transactions.Count == 0)
        {
            Console.WriteLine("No transactions exists.");
            return;
        }

        var sortedByDate = SortTransactionsByDate(transactions);

        // If logged in as admin or debug. Will show "deleted" transactions.
        if (IsAdmin)
        {
            AdminListTransactions(sortedByDate, entriesBeforeBreak);
            return;
        }

        int count = 0;
        foreach (TransactionEntry entry in sortedByDate)
        {
            count++;
            entry.Display();
            if (count % entriesBeforeBreak == 0)
            {
                Console.WriteLine("Press escape to stop. Any other key to keep going.");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    return;
                }
                Console.Clear();
            }
        }
    }

    // Standard is sorted descending date.
    public void ListUnsorted(List<TransactionEntry> transactions)
    {
        AdminListTransactions(transactions, 100);
    }

    // Admin list shows deleted transactions with ToString instead of Display.
    private void AdminListTransactions(List<TransactionEntry> transactions, int entriesBeforeBreak)
    {
        int count = 0;
        foreach (TransactionEntry entry in transactions)
        {
            count++;
            Console.WriteLine(entry.ToString());
            if (count % entriesBeforeBreak == 0)
            {
                Console.WriteLine("Press escape to stop. Any other key to keep going.");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    return;
                }
                Console.Clear();
            }
        }
    }

    public List<TransactionEntry> SortTransactionsByDate(List<TransactionEntry> transactions)
    {
        List<TransactionEntry> sortedList = new List<TransactionEntry>(transactions);
        sortedList.Sort((x, y) => DateTime.Compare(y.Date, x.Date));
        return sortedList;
    }
}
