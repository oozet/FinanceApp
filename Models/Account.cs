public class Account
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AccountNumber { get; set; }
    public long BalanceInMinorUnit { get; set; }
    public AccountType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
}

public enum AccountType
{
    Personal,
    Savings,
    Business,
}
