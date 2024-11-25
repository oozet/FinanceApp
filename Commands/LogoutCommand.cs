public class LogoutCommand : Command
{
    // Just a simple command to switch menu between user, admin and debug.

    public LogoutCommand(Program program)
        : base("Logout", "Type logout to log out the current user.", program) { }

    public override void Execute(string[] commandArgs)
    {
        program.AppDbContext.Logout();
        program.MenuManager.SetMenu(new NoUserMenu(program));
    }
}
