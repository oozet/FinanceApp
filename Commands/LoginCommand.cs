public class LoginCommand : Command
{
    // Just a simple command to switch menu between user, admin and debug.

    public LoginCommand(Program program)
        : base("Login", "Type 'login admin' or 'login debug' for special commands!", program) { }

    public override void Execute(string[] commandArgs)
    {
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
