using System.Text.Json;

// Derived class that adds funtionality to save to and load from a json file.
public class FileTransactionManager : ListTransactionManager
{
    protected string filePath = "data/financeData.json";

    public FileTransactionManager()
    {
        ReadFromFile();
    }

    // Sends the data to base.Add() then saves the List<TransactionEntry> to file.
    public override TransactionEntry CreateTransaction(float amount, TransactionType type)
    {
        //Also using base class function.
        var entry = base.CreateTransaction(amount, type);
        return entry;
    }

    // For testing purposes
    public override void AddTransaction(TransactionEntry transaction)
    {
        base.AddTransaction(transaction);
        SaveToFile();
    }

    // Sends the data to base.Remove() then saves the List<TransactionEntry> to file.
    public override void RemoveEntry(Guid uid)
    {
        // Also using base class function.
        base.RemoveEntry(uid);
        SaveToFile();
    }

    public override void RemoveTransaction(Guid uid)
    {
        base.RemoveTransaction(uid);
        SaveToFile();
    }

    private void SaveToFile()
    {
        // Learned you could serialize the whole list at once...
        // string jsonString = "";
        // foreach (TransactionEntry item in transactions)
        // {
        //     jsonString += JsonSerializer.Serialize(item) + ',';
        // }
        // jsonString = '[' + jsonString.TrimEnd(',') + ']';

        string jsonString = JsonSerializer.Serialize(transactions);
        // Create or Overwrite 'fileName'
        File.WriteAllText(filePath, jsonString);
    }

    private void ReadFromFile()
    {
        if (!File.Exists(filePath))
        {
            return;
        }
        using FileStream openStream = File.OpenRead(filePath);
        transactions =
            JsonSerializer.Deserialize<List<TransactionEntry>>(openStream)
            ?? new List<TransactionEntry>();
    }

    public override List<TransactionEntry> Populate(int count)
    {
        var newTransactions = base.Populate(count);
        SaveToFile();
        return newTransactions;
    }
}
