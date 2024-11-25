public class UserMenu : Menu
{
    public UserMenu(Program program)
        : base(program)
    {
        // AddCommand() in baseclass 'Menu'.
        AddCommand(new LogoutCommand(program));
        AddCommand(new DepositCommand(program));
        AddCommand(new WithdrawCommand(program));
        AddCommand(new BalanceCommand(program));
        AddCommand(new ListTransactionsCommand(program));
        AddCommand(new FilterTransactionsCommand(program));
    }

    public override void Display()
    {
        UIService.DisplayLogo();
        UIService.DisplayUserInfo(program.AppDbContext.currentUser!);
    }
}
