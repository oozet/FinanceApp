public class FilterTransactionsCommand : Command
{
    public FilterTransactionsCommand(Program program)
        : base(
            "Filter",
            "You may filter by year, month, week or day. Example: filter week 31.",
            program
        ) { }

    public override void Execute(string[] commandArgs)
    {
        if (commandArgs.Length != 3)
        {
            throw new InvalidCommandException(
                "Usage: filter 'type' 'value'.\nExample: filter year 2023"
            );
        }

        string[] filterCommand = [commandArgs[1], commandArgs[2]];
        FilterTransactionsService filterTransactionsService = new FilterTransactionsService(
            program
        );

        List<TransactionEntry> transactions = filterTransactionsService.FilterTransactions(
            filterCommand
        );

        if (transactions.Count.Equals(0))
        {
            Console.WriteLine("Your filter didn't return any results.");
        }
        else
        {
            int entriesBeforeBreak = 10;
            Console.WriteLine(
                "Input number of transactions to show at once(Default: 10, Max: 100):"
            );
            if (int.TryParse(Console.ReadLine(), out int result))
                entriesBeforeBreak = Math.Clamp(result, 1, 100);

            TransactionDisplayService transactionDisplayService = new();
            bool IsAdmin = program.MenuManager.GetCurrentMenu() is AdminMenu;
            transactionDisplayService.ListTransactions(entriesBeforeBreak, transactions, IsAdmin);

            // (float depositAmount, float withdrawalAmount) =
            //     program.TransactionManager.GetTotalPerType(transactions);
            // Console.WriteLine(
            //     $"You've made ${depositAmount} and spent ${withdrawalAmount}. A net balance of ${depositAmount - withdrawalAmount} during this time. "
            // );
        }

        UIService.DisplayWaitForEnter();
        program.MenuManager.GetCurrentMenu().Display();
    }
}
