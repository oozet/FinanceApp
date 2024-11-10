// Not an interface to have Exit and LoginCommand for all menus and hardcoded HelpCommand.
// Could have used interface and a default menu that all other menus inherited from.

using System.Text;

public abstract class Menu
{
    private List<Command> commands = new List<Command>();
    protected Program program;

    public Menu(Program program)
    {
        this.program = program;
        AddCommand(new ExitCommand(program));
        AddCommand(new LoginCommand(program));
    }

    protected void AddCommand(Command command)
    {
        commands.Add(command);
    }

    public void TryExecuteCommand(string input)
    {
        string[] commandArgs = input.Split(" ");
        string commandName = commandArgs[0];

        // Special case for help to list all commands.
        if (commandName.Equals("help"))
        {
            StringBuilder sb = new StringBuilder("List of commands: \n");
            foreach (Command command in commands)
            {
                sb.Append($"{command.Name} - {command.Description}\n");
            }
            Console.WriteLine(sb.ToString());
            return;
        }

        foreach (Command command in commands)
        {
            if (command.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    command.Execute(commandArgs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

    public abstract void Display();
}
