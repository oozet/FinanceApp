public class Program
{
    public ITransactionManager TransactionManager { get; set; }

    public MenuManager MenuManager { get; set; }
    public bool running = true;

    public Program()
    {
        this.MenuManager = new MenuManager(new UserMenu(this));
        this.TransactionManager = new FileTransactionManager();
    }

    static void Main(string[] args)
    {
        Program program = new Program();
        program.TransactionManager.OnProgramLoad();

        while (program.running)
        {
            string input = Console.ReadLine()!;
            program.MenuManager.GetCurrentMenu().TryExecuteCommand(input);
        }
    }
}
