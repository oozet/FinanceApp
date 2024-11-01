using System.Globalization;

public class FilterTransactionsService
{
    private Program program;

    public FilterTransactionsService(Program program)
    {
        this.program = program;
    }

    // Filter transaction based on user input. Requests additional input
    // from user to get correct year and month. Filter methods return a
    // predicate that is passed on to FilterTransactions in TransactionManager
    // which returns a list of filtered entries.
    public List<TransactionEntry> FilterTransactions(string[] filterCommand)
    {
        Predicate<TransactionEntry> predicate;
        string filterType = filterCommand[0];
        string filterValue = filterCommand[1];
        string year = "";
        string month;

        if (filterType != "year")
        {
            Console.Write("Enter year: ");
            year = StringInput();
        }

        switch (filterType)
        {
            case "year":
                Console.WriteLine("Filter by year.");
                predicate = FilterByYear(filterValue);
                break;
            case "month":
                Console.WriteLine("Filter by month.");
                predicate = FilterByMonth(filterValue, year);
                break;
            case "week":
                Console.WriteLine("Filter by week.");
                predicate = FilterByWeekNumber(filterValue, year);
                break;
            case "day":
                Console.Write("Enter month: ");
                month = StringInput();
                predicate = FilterByDay(filterValue, year, month);
                break;
            case "deposit":
                predicate = FilterByType(filterValue);
                break;
            case "withdrawal":
                predicate = FilterByType(filterValue);
                break;

            default:
                throw new InvalidCommandException(
                    "Wrong sorting type. Valid inputs: year, month, week, day."
                );
        }
        return program.TransactionManager.FilterTransactions(predicate);
    }

    // Just a small method to get a valid string as input.
    private string StringInput()
    {
        string str;
        while (true)
        {
            str = Console.ReadLine()!;
            if (str != null && str.Length > 0)
                return str;
        }
    }

    private Predicate<TransactionEntry> FilterByYear(string filterValue)
    {
        if (!int.TryParse(filterValue, out int year))
        {
            throw new ArgumentException("Invalid year");
        }
        return x => x.Date.Year == year;
    }

    // Filter transactions by month. Command: 'filter month (monthName or monthnumber)' requires second input of 'year'
    // from FilterTransactionsService.
    private Predicate<TransactionEntry> FilterByMonth(string filterValue, string year)
    {
        Predicate<TransactionEntry> predicate;
        // If input is a number.
        if (int.TryParse(filterValue, out int month))
        {
            if (1 >= month && month >= 12)
            {
                throw new ArgumentOutOfRangeException("Invalid number. Must be between 1 and 12.");
            }
            predicate = x => x.Date.Month == month;
            return x => predicate(x) && FilterByYear(year)(x);
        }

        // Trying to parse the string to enum.Month second parameter 'true'
        // makes it case-insensitive.
        if (!Enum.TryParse<Month>(filterValue.ToLower(), true, out Month monthEnum))
        {
            throw new ArgumentException("Invalid month name.");
        }
        predicate = x => x.Date.Month == (int)monthEnum;

        return x => predicate(x) && FilterByYear(year)(x);
    }

    // Filter transactions by week. Command: 'filter week weeknumber' requires second input of 'year'
    // from FilterTransactionsService.
    public Predicate<TransactionEntry> FilterByWeekNumber(string filterValue, string year)
    {
        Predicate<TransactionEntry> predicate;
        if (int.TryParse(filterValue, out int weekNumber))
        {
            predicate = x =>
            {
                // Default calendar of user.
                Calendar calendar = CultureInfo.CurrentCulture.Calendar;
                // Rule that determines first week of the year depending on .
                CalendarWeekRule weekRule = CultureInfo
                    .CurrentCulture
                    .DateTimeFormat
                    .CalendarWeekRule;
                // Determine what day a week starts.
                DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

                // Give weeknumber based on iterated Date used in prediction.
                int dateWeekNumber = calendar.GetWeekOfYear(x.Date, weekRule, firstDayOfWeek);
                // Compare to week number given by user.
                return dateWeekNumber == weekNumber;
            };
        }
        else
        {
            throw new ArgumentException("Invalid week number. Enter a number between 1 and 53.");
        }
        return x => predicate(x) && FilterByYear(year)(x);
    }

    // Filter transactions by day of the month. Command: 'filter day dayofmonth'.  requires additional
    // input of 'year' and 'month' from FilterTransactionsService.
    private Predicate<TransactionEntry> FilterByDay(string filterValue, string year, string month)
    {
        if (int.TryParse(filterValue, out int day))
        {
            // Allow to filter day 31 of any month because it's easier. Will just return no results.
            if (1 > day && day >= 31)
            {
                throw new DayNumberOutOfRangeException(
                    "Invalid day: enter a number between 1 and 31"
                );
            }
            Predicate<TransactionEntry> predicate = x => x.Date.Day == int.Parse(filterValue);
            return x => predicate(x) && FilterByMonth(month, year)(x);
        }
        else
        {
            throw new InvalidNumberException("Invalid number: enter a number between 1 and 31");
        }
    }

    private Predicate<TransactionEntry> FilterByType(string filterValue)
    {
        if (!Enum.TryParse(filterValue, out TransactionType typeString))
        {
            throw new InvalidTypeException("Invalid transaction type.");
        }
        return x => x.Type == typeString;
    }
}
