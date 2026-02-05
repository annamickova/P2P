using P2P.DataLayer;
using P2P.Utils;
using P2P.Monitoring;

namespace P2P.NetworkLayer;

public class AwCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        MonitoringState.SetLastCommand("AW");
        MonitoringState.IncrementCommands();

        if (args.Length != 2)
        {
            string error = "Incorrect amount of parameters.";
            MonitoringState.IncrementErrors(error);
            return Task.FromResult("ER " + error);
        }

        try
        {
            int accountId = CommandHelper.ParseAccountId(args[0]);
            long amount = CommandHelper.ParseAmount(args[1]);
            
            string ip = args[0].Split("/")[1];
            if (ip != CommandHelper.MyIp)
            {
                Node node = new(ip);
                return node.SendRequestAsync($"AW {accountId}/{ip} {amount}")!;
            }
            
            Logger.Info($"Withdraw request: account {accountId}, amount {amount}");
            var dao = BankStorageSingleton.Instance.Dao;
            var account = dao.GetById(accountId);

            if (account == null)
            {
                string error = "Account doesn't exist.";
                MonitoringState.IncrementErrors(error);
                Logger.Error($"Withdraw request: account {accountId} not found");
                return Task.FromResult("ER " + error);
            }

            if (account.Balance < amount)
            {
                string error = "Not enough money saved.";
                MonitoringState.IncrementErrors(error);
                Logger.Warning($"Withdraw request: account {accountId} not balanced.");
                return Task.FromResult("ER " + error);
            }

            account.Balance -= amount;
            if (dao.Update(account))
            {
                Logger.Info($"Withdraw successful: account {accountId}, new balance {account.Balance}");
                return Task.FromResult("AW");
            }
            
            string updateError = "Error while withdrawing.";
            MonitoringState.IncrementErrors(updateError);
            return Task.FromResult("ER " + updateError);
        }
        catch (Exception exception)
        {
            MonitoringState.IncrementErrors(exception.Message);
            return Task.FromResult($"ER {exception.Message}");
        }
    }
}