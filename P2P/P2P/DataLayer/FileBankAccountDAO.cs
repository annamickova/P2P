namespace P2P.DataLayer;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class FileBankAccountDao : IGenericDao<BankAccount>
{
    private const string DbFile = "bank_db.json";
    private ConcurrentDictionary<int, BankAccount> _storage = new();

    public void Initialize()
    {
        if (!File.Exists(DbFile))
        {
            using (var fileStream = File.Open(DbFile, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                //ignored
            }
        }
        
        if (File.Exists(DbFile))
        {
            using (var fileStream = File.Open(DbFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                //ignored
            }
            
            var json = File.ReadAllText(DbFile);
            var list = JsonSerializer.Deserialize<List<BankAccount>>(json);
            if (list != null)
            {
                foreach (var item in list) _storage.TryAdd(item.AccountNumber, item);
            }
        }
    }

    private void SaveToFile()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_storage.Values.ToList(), options);
        File.WriteAllText(DbFile, json);
    }

    public List<BankAccount> GetAll() => _storage.Values.ToList();

    public BankAccount? GetById(int id)
    {
        _storage.TryGetValue(id, out var acc);
        return acc;
    }

    public bool Save(BankAccount bankAccount)
    {
        if (_storage.TryAdd(bankAccount.AccountNumber, bankAccount))
        {
            SaveToFile();
            return true;
        }
        return false;
    }

    public bool Update(BankAccount bankAccount)
    {
        _storage[bankAccount.AccountNumber] = bankAccount;
        SaveToFile();
        return true;
    }

    public bool Delete(int id)
    {
        if (_storage.TryRemove(id, out _))
        {
            SaveToFile();
            return true;
        }
        return false;
    }
}