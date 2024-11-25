public class NoUserMenu : Menu
{
    public NoUserMenu(Program program)
        : base(program)
    {
        // AddCommand() in baseclass 'Menu'.
        AddCommand(new LoginCommand(program));
        AddCommand(new CreateUserCommand(program));
    }

    public override void Display()
    {
        UIService.DisplayLogo();
        UIService.DisplayNoUserInfo();
    }
}
