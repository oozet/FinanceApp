public class InsufficientBalanceException : Exception
{
    public InsufficientBalanceException(string message)
        : base(message) { }
}

public class InvalidNumberException : Exception
{
    public InvalidNumberException(string message)
        : base(message) { }
}

public class InvalidTypeException : Exception
{
    public InvalidTypeException(string message)
        : base(message) { }
}

public class InvalidCommandException : Exception
{
    public InvalidCommandException(string message)
        : base(message) { }
}

public class DayNumberOutOfRangeException : Exception
{
    public DayNumberOutOfRangeException(string message)
        : base(message) { }
}

public class InvalidUserException : Exception
{
    public InvalidUserException(string message)
        : base(message) { }
}
