using P2P.DataLayer;

namespace P2P.NetworkLayer;

public class BnCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        try
        {
            var allAccounts = BankStorageSingleton.Instance.Dao.GetAll();
            
            return Task.FromResult($"BN {allAccounts.Count}");
        }
        catch (Exception exception)
        {
            return Task.FromResult($"ER Error while calculating the amount of clients: {exception.Message}");
        }
    }
}