public class LoginCommand : Command
{
    // Just a simple command to switch menu between user, admin and debug.

    public LoginCommand(Program program)
        : base("Login", "Type 'login admin' or 'login debug' for special commands!", program) { }

    public override async void Execute(string[] commandArgs)
    {
        if (commandArgs.Length < 3)
        {
            Console.WriteLine("Usage: 'login username password'");
            return;
        }

        string username = commandArgs[1];
        string password = commandArgs[2];
        if (!await program.AppDbContext.SetUser(username, password))
        {
            Console.WriteLine("Invalid username or password.");
            return;
        }

        if (commandArgs[1].Equals("debug"))
        {
            program.MenuManager.SetMenu(new DebugMenu(program));
        }
        else if (commandArgs[1].Equals("admin"))
        {
            program.MenuManager.SetMenu(new AdminMenu(program));
        }
        else
        {
            program.MenuManager.SetMenu(new UserMenu(program));
        }
    }
}
