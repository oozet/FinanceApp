public class Program
{
    public ITransactionManager TransactionManager { get; set; }
    public AppDbContext AppDbContext { get; set; }

    public MenuManager MenuManager { get; set; }
    public bool running = true;

    public Program()
    {
        string connectionString =
            "Host=localhost;Username=postgres;Password=password;Database=exercises";
        this.AppDbContext = new AppDbContext(connectionString);
        this.MenuManager = new MenuManager(new NoUserMenu(this));
        this.TransactionManager = new FileTransactionManager();
    }

    static void Main(string[] args)
    {
        Program program = new Program();
        program.TransactionManager.OnProgramLoad();
        DatabaseService.EnsureTablesExists(program.AppDbContext);

        while (program.running)
        {
            string input = Console.ReadLine()!;
            program.MenuManager.GetCurrentMenu().TryExecuteCommand(input);
        }
    }
}

// Without hardcoding the connectionstring
// using Microsoft.Extensions.Configuration;

// // Build configuration
// var configuration = new ConfigurationBuilder()
//     .SetBasePath(Directory.GetCurrentDirectory())
//     .AddJsonFile("appsettings.json")
//     .Build();

// // Retrieve the connection string
// string connectionString = configuration.GetConnectionString("DefaultConnection");
