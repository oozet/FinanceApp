using Xunit;

namespace Finance.Tests;

public class AddRemoveTest
{
    [Fact]
    public void AddTransaction_ShouldAddTransaction()
    {
        var transactionManager = new TransactionManager();
        var transaction = new TransactionEntry
        {
            Date = DateTime.Now,
            Amount = 100,
            Uid = transactionManager.UidCounter,
            Type = TransactionType.Deposit,
        };
        transactionManager.AddTransaction(transaction);
        Assert.Contains(transaction, transactionManager.GetAllTransactions());
    }

    [Fact]
    public void AdminRemoveTransaction_ShouldRemoveTransaction()
    {
        // Arrange
        var transactionManager = new TransactionManager();
        var transaction = new TransactionEntry
        {
            Date = DateTime.Now,
            Amount = 100,
            Uid = transactionManager.UidCounter,
            Type = TransactionType.Deposit,
        };
        transactionManager.AddTransaction(transaction);

        // Act
        transactionManager.RemoveTransaction(transaction.Uid);

        // Assert
        Assert.DoesNotContain(transaction, transactionManager.GetAllTransactions());
    }

    [Fact]
    public void RemoveTransaction_ShouldUpdateTransaction()
    {
        // Arrange
        var transactionManager = new TransactionManager();
        var transaction = new TransactionEntry
        {
            Date = DateTime.Now,
            Amount = 100,
            Uid = transactionManager.UidCounter,
            Type = TransactionType.Deposit,
        };
        transactionManager.AddTransaction(transaction);

        // Act
        transactionManager.RemoveEntry(transaction.Uid);

        // Assert
        var updatedTransaction = transactionManager.GetTransaction(transaction.Uid);
        Assert.NotNull(updatedTransaction.DeletedAt);
        Assert.Equal(DateTime.Now.Date, updatedTransaction.DeletedAt?.Date);
    }
}
