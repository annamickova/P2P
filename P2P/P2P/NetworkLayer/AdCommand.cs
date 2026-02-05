using P2P.DataLayer;
using P2P.Utils;
using P2P.Monitoring;

namespace P2P.NetworkLayer;

public class AdCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        if (args.Length != 2)
        {
            string error = "Incorrect amount of parameters.";
            MonitoringState.IncrementErrors(error);
            return Task.FromResult("ER Incorrect amount of parameters.");
        }

        try
        {
            int accountId = CommandHelper.ParseAccountId(args[0]);

            long amount = CommandHelper.ParseAmount(args[1]);

            string ip = args[0].Split("/")[1];
            if (ip != CommandHelper.MyIp)
            {
                Node node = new(ip);
                return node.SendRequestAsync($"AD {accountId}/{ip} {amount}")!;
            }

            var dao = BankStorageSingleton.Instance.Dao;
            var account = dao.GetById(accountId);

            if (account == null)
            {
                string error = "Account doesn't exist.";
                MonitoringState.IncrementErrors(error);
                Logger.Error("Account doesn't exist.");
                return Task.FromResult("ER Account doesn't exist.");
            }

            account.Balance += amount;
            
            if (dao.Update(account))
            {
                return Task.FromResult("AD");
            }
            string updateError = "Error while depositing.";
            MonitoringState.IncrementErrors(updateError);
            return Task.FromResult("ER Error while depositing.");
        }
        catch (Exception exception)
        {
            MonitoringState.IncrementErrors(exception.Message);
            return Task.FromResult($"ER {exception.Message}");
        }
    }
}