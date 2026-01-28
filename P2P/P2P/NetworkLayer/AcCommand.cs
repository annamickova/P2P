using P2P.DataLayer;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class AcCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        var rnd = new Random();
        int newId;
        int attempts = 0;
        
        do 
        {
            newId = rnd.Next(10000, 100000);
            attempts++;
            if (attempts > 100) return Task.FromResult("ER Can't generate unique ID.");
        } 
        while (BankStorageSingleton.Instance.Dao.GetById(newId) != null);

        var account = new BankAccount { AccountNumber = newId, Balance = 0 };
        
        if (BankStorageSingleton.Instance.Dao.Save(account))
        {
            return Task.FromResult($"AC {newId}/{CommandHelper.MyIp}");
        }
        
        return Task.FromResult("ER Error while saving an account.");
    }
}