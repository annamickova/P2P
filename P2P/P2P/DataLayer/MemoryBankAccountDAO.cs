namespace P2P.DataLayer;

using System.Collections.Concurrent;

public class MemoryBankAccountDao : IGenericDao<BankAccount>
{
    private readonly ConcurrentDictionary<int, BankAccount> _storage = new();

    public void Initialize(){}

    public List<BankAccount> GetAll() => _storage.Values.ToList();

    public BankAccount? GetById(int id)
    {
        _storage.TryGetValue(id, out var account);
        return account;
    }

    public bool Save(BankAccount bankAccount) => _storage.TryAdd(bankAccount.AccountNumber, bankAccount);

    public bool Update(BankAccount bankAccount)
    {
        _storage[bankAccount.AccountNumber] = bankAccount;
        return true;
    }

    public bool Delete(int id) => _storage.TryRemove(id, out _);
}