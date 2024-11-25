public class CreateUserCommand : Command
{
    // Just a simple command to switch menu between user, admin and debug.

    public CreateUserCommand(Program program)
        : base("Create", "Type 'Create' then username", program) { }

    public override async void Execute(string[] commandArgs)
    {
        if (commandArgs.Length < 2)
        {
            Console.WriteLine("Command too short! Type 'Create user' then username");
            return;
        }

        string username = commandArgs[1];
        Console.WriteLine("Enter password:");
        string password = Console.ReadLine();

        if (!await program.AppDbContext.CreateUser(username, password))
        {
            Console.WriteLine("Username already in use.");
            return;
        }
        Console.WriteLine("Success?");
    }
}
