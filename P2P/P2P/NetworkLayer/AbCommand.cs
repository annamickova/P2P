using P2P.DataLayer;
using P2P.Utils;
using P2P.Monitoring;

namespace P2P.NetworkLayer;

public class AbCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        if (args.Length != 1)
        {
            string error = "Incorrect amount of parameters.";
            MonitoringState.IncrementErrors(error);
            return Task.FromResult("ER Incorrect amount of parameters.");
        }

        try
        {
            int accountId = CommandHelper.ParseAccountId(args[0]);
            
            string ip = args[0].Split("/")[1];
            if (ip != CommandHelper.MyIp)
            {
                Node node = new(ip);
                return node.SendRequestAsync($"AB {accountId}/{ip}")!;
            }
            
            var account = BankStorageSingleton.Instance.Dao.GetById(accountId);
            if (account == null)
            {
                string error = "Account doesn't exist.";
                MonitoringState.IncrementErrors(error);
                return Task.FromResult("ER Account doesn't exist.");
            }
            

            Logger.Debug($"Balance requested for account {accountId}");
            return Task.FromResult($"AB {account.Balance}");
        }
        catch (Exception exception)
        {
            MonitoringState.IncrementErrors(exception.Message);
            return Task.FromResult($"ER {exception.Message}");
        }
    }
}