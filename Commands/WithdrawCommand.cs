public class WithdrawCommand : Command
{
    public WithdrawCommand(Program program)
        : base(
            "Withdraw",
            "Type withdraw followed by the amount to make a withdrawal. Make sure you have enough in the bank!",
            program
        ) { }

    public override void Execute(string[] commandArgs)
    {
        if (commandArgs.Length != 2)
        {
            throw new InvalidCommandException("Usage: withdraw 'amount'");
        }
        if (!float.TryParse(commandArgs[1], out var result))
        {
            throw new InvalidNumberException("Not a valid number.");
        }

        // Could skip the check and just do Absolute value on all entries.
        if (result < 0)
        {
            result = Math.Abs(result);
        }

        float currentBalance = program.TransactionManager.GetTotal();
        if (currentBalance - result < 0)
        {
            throw new InsufficientBalanceException("Insufficient balance on account.");
        }

        var transactionManager = program.TransactionManager;

        transactionManager.AddTransaction(
            transactionManager.CreateTransaction(result, TransactionType.Withdrawal)
        );
    }
}
