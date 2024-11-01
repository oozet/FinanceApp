public class LoginCommand : Command
{
    // Just a simple command to switch menu between user and admin.
    // Disregards second string and checks if third string is "pass"
    // to login as admin for simplicity.

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
