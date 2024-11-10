public class ListTransactionsCommand : Command
{
    // Lists all transactions in TransactionManager transactions five at a time at default.
    // Then listens for escape press to stop listing. If logged in as admin or debug, shows deleted transactions as well.
    public ListTransactionsCommand(Program program)
        : base("List", "Shows all transactions.", program) { }

    public override void Execute(string[] commandArgs)
    {
        var transactionManager = program.TransactionManager;
        List<TransactionEntry> transactions = transactionManager.GetAllTransactions();
        int entriesBeforeBreak = 20;
        if (commandArgs.Length == 2)
        {
            if (int.TryParse(commandArgs[1], out int result))
                entriesBeforeBreak = Math.Clamp(result, 1, 100);
        }

        TransactionDisplayService transactionDisplayService = new();
        bool IsAdmin = program.MenuManager.GetCurrentMenu() is AdminMenu or DebugMenu;
        transactionDisplayService.ListTransactions(entriesBeforeBreak, transactions, IsAdmin);

        Console.WriteLine("Balance: " + float.Round(transactionManager.GetTotal(), 2));

        UIService.DisplayWaitForEnter();
        program.MenuManager.GetCurrentMenu().Display();
    }
}
