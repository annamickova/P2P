using P2P.DataLayer;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class BaCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        try
        {
            Logger.Debug("Total bank amount requested.");

            var allAccounts = BankStorageSingleton.Instance.Dao.GetAll();
            
            long totalAmount = allAccounts.Sum(bankAccount => bankAccount.Balance);

            return Task.FromResult($"BA {totalAmount}");
        }
        catch (Exception exception)
        {
            Logger.Error($"Error calculating the total amount of money: {exception.Message}");
            return Task.FromResult($"ER Error while calculating the total amount of money: {exception.Message}");
        }
    }
}