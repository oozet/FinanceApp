using System.Text.Json;
using Xunit;

namespace Finance.Tests;

public class JsonValidationTest
{
    [Fact]
    public void JsonFile_ShouldBeCorrectlyFormatted()
    {
        string dataDirectory = "Data";
        Directory.CreateDirectory(dataDirectory);
        var transactionManager = new FileTransactionManager();
        var transaction = new TransactionEntry
        {
            Date = DateTime.Now,
            AmountInMinorUnit = 10000,
            Uid = transactionManager.UidCounter,
            Type = TransactionType.Deposit,
        };
        var transaction2 = new TransactionEntry
        {
            Date = DateTime.Now,
            AmountInMinorUnit = 20000,
            Uid = transactionManager.UidCounter,
            Type = TransactionType.Withdrawal,
        };

        transactionManager.AddTransaction(transaction);
        transactionManager.AddTransaction(transaction2);

        string json = File.ReadAllText(Path.Combine(dataDirectory, "financeData.json"));
        Assert.True(IsValidJson(json));

        Directory.Delete(dataDirectory, true);
    }

    private bool IsValidJson(string json)
    {
        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
