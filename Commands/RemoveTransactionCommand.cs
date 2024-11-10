public class RemoveTransactionCommand : Command
{
    public RemoveTransactionCommand(Program program)
        : base(
            "Remove",
            "Sets RemovedAt to the current date hiding the entry from list and filter.",
            program
        ) { }

    public override void Execute(string[] commandArgs)
    {
        if (commandArgs.Length != 2)
        {
            throw new InvalidCommandException("Command usage: remove 'Uid'.");
        }
        if (int.TryParse(commandArgs[1], out int result))
        {
            program.TransactionManager.RemoveEntry(result);
        }
        else
        {
            throw new InvalidNumberException("Uid must be a number (example: remove 114).");
        }
    }
}

public class PurgeTransactionCommand : Command
{
    public PurgeTransactionCommand(Program program)
        : base(
            "Purge",
            "Permanently removes an entry from the list of transactions. Only for debugging!",
            program
        ) { }

    public override void Execute(string[] commandArgs)
    {
        if (commandArgs.Length != 2)
        {
            throw new InvalidCommandException("Command usage: remove 'Uid'.");
        }
        if (int.TryParse(commandArgs[1], out int result))
        {
            program.TransactionManager.RemoveTransaction(result);
        }
        else
        {
            throw new InvalidNumberException("Uid must be a number (example: remove 114).");
        }
    }
}
