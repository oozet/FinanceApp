public class AdminMenu : UserMenu
{
    public AdminMenu(Program program)
        : base(program)
    {
        // AddCommand() in baseclass 'Menu'.
        AddCommand(new PopulateCommand(program));
        AddCommand(new RemoveTransactionCommand(program));
    }

    public override void Display()
    {
        UIService.DisplayLogo();
        UIService.DisplayAdminInfo();
    }
}
