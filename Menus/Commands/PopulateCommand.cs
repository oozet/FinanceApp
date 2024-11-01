public class PopulateCommand : Command
{
    public PopulateCommand(Program program)
        : base("Populate", "Lets you create a bunch of random transactions. Free money?!", program)
    { }

    public override void Execute(string[] commandArgs)
    {
        if (commandArgs.Length != 2)
        {
            throw new InvalidCommandException(
                "Usage: populate 'count'. Where count is the amount of entries."
            );
        }
        if (!int.TryParse(commandArgs[1], out var result))
        {
            throw new InvalidNumberException("input must be a valid number.");
        }
        if (result < 1)
        {
            throw new ArgumentException("count must be more than 1.");
        }
        Console.WriteLine("", result);
        program.TransactionManager.Populate(result);
    }
}
