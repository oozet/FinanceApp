public class UIService
{
    public static void DisplayLogo()
    {
        // Console.Clear() fails in Testing. Try/Catch used as workaround.
        try
        {
            Console.Clear();
        }
        catch { }

        string finance = """
            -------------------------------------------------------------------------------------
            ░▒▓████████▓▒░▒▓█▓▒░▒▓███████▓▒░ ░▒▓██████▓▒░░▒▓███████▓▒░ ░▒▓██████▓▒░░▒▓████████▓▒░ 
            ░▒▓█▓▒░      ░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░         
            ░▒▓██████▓▒░ ░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓████████▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓██████▓▒░   
            ░▒▓█▓▒░      ░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░        
            ░▒▓█▓▒░      ░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░░▒▓██████▓▒░░▒▓████████▓▒░
            -------------------------------------------------------------------------------------
            """;
        Console.WriteLine(finance);
    }

    public static void DisplayUserInfo()
    {
        Console.WriteLine(
            """
               Welcome to your personal finance application. Type help for further assistance.
            -------------------------------------------------------------------------------------
                            | Deposit | Withdraw | Balance | Filter | List |     
            -------------------------------------------------------------------------------------
            """
        );
    }

    public static void DisplayAdminInfo()
    {
        Console.WriteLine(
            """
                          Welcome Administrator. Type help for further assistance.
            -------------------------------------------------------------------------------------
                    | Populate | Deposit | Withdraw | Remove | Balance | Filter | List |     
            -------------------------------------------------------------------------------------
            """
        );
    }

    public static void DisplayDebugInfo()
    {
        Console.WriteLine(
            """
                          Welcome to debug mode. Type help for further assistance.
            -------------------------------------------------------------------------------------
            Populate | Deposit | Withdraw | Remove | Purge | Get | Count | Balance | Filter | List     
            -------------------------------------------------------------------------------------
            """
        );
    }

    public static void DisplayWaitForEnter()
    {
        Console.WriteLine("Press enter to return to menu.");
        Console.ReadLine();
    }

    public virtual void ClearConsole()
    {
        Console.Clear();
    }
}
