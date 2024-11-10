public class ExitCommand : Command
{
    public ExitCommand(Program program)
        : base("Exit", "Turns your Finance application off.", program) { }

    public override void Execute(string[] commandArgs)
    {
        program.running = false;
    }
}
