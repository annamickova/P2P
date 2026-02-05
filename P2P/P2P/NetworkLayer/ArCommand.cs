using P2P.DataLayer;
using P2P.Utils;
using P2P.Monitoring;

namespace P2P.NetworkLayer;

public class ArCommand : ICommand
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
            var dao = BankStorageSingleton.Instance.Dao;
            var account = dao.GetById(accountId);
            
            string ip = args[0].Split("/")[1];
            if (ip != CommandHelper.MyIp)
            {
                Node node = new(ip);
                return node.SendRequestAsync($"AR {accountId}/{ip}")!;
            }

            Logger.Info($"Delete account request: {accountId}");

            if (account == null)
            {
                string error = "Account doesn't exist.";
                MonitoringState.IncrementErrors(error);
                Logger.Error($"Account {accountId} not found.");
                return Task.FromResult("ER Account doesn't exist.");
            }

            if (account.Balance != 0)
            {
                string error = "Cannot delete account when the amount of money is greater than 0.";
                MonitoringState.IncrementErrors(error);
                Logger.Warning($"Cannot delete account {accountId} – balance not zero");
                return Task.FromResult("ER Cannot delete account when the amount of money is greater than 0.");
            }

            if (dao.Delete(accountId))
            {
                Logger.Info($"Account deleted: {accountId}");
                return Task.FromResult("AR");
            }
            
            string deleteError = "Error while deleting.";
            MonitoringState.IncrementErrors(deleteError);
            Logger.Error($"Failed to delete account {accountId}");
            return Task.FromResult("ER Error while deleting.");
        }
        catch (Exception exception)
        {
            MonitoringState.IncrementErrors(exception.Message);
            return Task.FromResult($"ER {exception.Message}");
        }
    }
}