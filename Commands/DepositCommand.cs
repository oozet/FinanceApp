public class DepositCommand : Command
{
    public DepositCommand(Program program)
        : base("Deposit", "Type deposit followed by the amount to make a deposit.", program) { }

    public override void Execute(string[] commandArgs)
    {
        var transactionManager = program.TransactionManager;
        if (commandArgs.Length != 2)
        {
            throw new InvalidCommandException("Usage: deposit 'amount'");
        }
        if (!float.TryParse(commandArgs[1], out var result))
        {
            throw new InvalidNumberException("Not a valid number.");
        }
        if (result < 0)
        {
            throw new InvalidNumberException("You can't deposit a negative amount.");
        }

        transactionManager.AddTransaction(
            transactionManager.CreateTransaction(result, TransactionType.Deposit)
        );
    }
}
