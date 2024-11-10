public abstract class Command
{
    public string Name { get; init; }
    public string Description { get; init; }

    protected Program program;

    public Command(string name, string description, Program program)
    {
        Name = name;
        Description = description;
        this.program = program;
    }

    public abstract void Execute(string[] commandArgs);
}
