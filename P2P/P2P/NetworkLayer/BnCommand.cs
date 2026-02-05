using P2P.DataLayer;
using P2P.Monitoring;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class BnCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        try
        {
            MonitoringState.SetLastCommand("BN");
            MonitoringState.IncrementCommands();
            Logger.Debug("Bank client count requested.");

            var allAccounts = BankStorageSingleton.Instance.Dao.GetAll();
            
            return Task.FromResult($"BN {allAccounts.Count}");
        }
        catch (Exception exception)
        {
            MonitoringState.IncrementErrors(exception.Message);
            Logger.Error($"Error while calculating the amount of clients: {exception.Message}");
            return Task.FromResult($"ER Error while calculating the amount of clients: {exception.Message}");
        }
    }
}