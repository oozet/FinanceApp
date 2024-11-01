public class DebugMenu : AdminMenu
{
    public DebugMenu(Program program)
        : base(program)
    {
        AddCommand(new PurgeTransactionCommand(program));
        AddCommand(new NoSortCommand(program));
        AddCommand(new GetEntry(program));
        AddCommand(new Count(program));
    }

    public override void Display()
    {
        UIService.DisplayLogo();
        UIService.DisplayDebugInfo();
    }
}

public class NoSortCommand : Command
{
    public NoSortCommand(Program program)
        : base("NoSort", "Display transactions without sorting by date.", program) { }

    public override void Execute(string[] commandArgs)
    {
        var transactionManager = program.TransactionManager;

        TransactionDisplayService transactionDisplayService = new();

        transactionDisplayService.ListUnsorted(transactionManager.GetAllTransactions());
    }
}

public class GetEntry : Command
{
    public GetEntry(Program program)
        : base("Get", "Prints a single transaction based on Uid", program) { }

    public override void Execute(string[] commandArgs)
    {
        int uid;
        var transactionManager = program.TransactionManager;
        if (commandArgs.Length < 2)
        {
            do Console.WriteLine("Get transactions with Uid: ");
            while (!int.TryParse(Console.ReadLine(), out uid));
        }
        else
        {
            uid = int.Parse(commandArgs[1]);
        }

        TransactionEntry? transaction = transactionManager.GetTransaction(uid);

        if (transaction != null)
            Console.WriteLine(transaction.ToString());
    }
}

public class Count : Command
{
    public Count(Program program)
        : base("Count", "Returns the total number of transactions.", program) { }

    public override void Execute(string[] commandArgs)
    {
        var transactionManager = program.TransactionManager;
        var transactions = transactionManager.GetAllTransactions();
        Console.WriteLine($"transactions contains : {transactions.Count} elements.");
    }
}
