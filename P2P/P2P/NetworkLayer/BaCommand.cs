using P2P.DataLayer;

namespace P2P.NetworkLayer;

public class BaCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        try
        {
            var allAccounts = BankStorageSingleton.Instance.Dao.GetAll();
            
            long totalAmount = allAccounts.Sum(bankAccount => bankAccount.Balance);

            return Task.FromResult($"BA {totalAmount}");
        }
        catch (Exception exception)
        {
            return Task.FromResult($"ER Chyba při výpočtu celkové sumy: {exception.Message}");
        }
    }
}