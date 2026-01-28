using P2P.DataLayer;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class ArCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        if (args.Length != 1) return Task.FromResult("ER Incorrect amount of parameters.");

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
                Logger.Error($"Account {accountId} not found.");
                return Task.FromResult("ER Account doesn't exist.");
            }

            if (account.Balance != 0)
            {
                Logger.Warning($"Cannot delete account {accountId} – balance not zero");
                return Task.FromResult("ER Cannot delete account when the amount of money is greater than 0.");
            }

            if (dao.Delete(accountId))
            {
                Logger.Info($"Account deleted: {accountId}");
                return Task.FromResult("AR");
            }
            
            Logger.Error($"Failed to delete account {accountId}");
            return Task.FromResult("ER Error while deleting.");
        }
        catch (Exception exception)
        {
            return Task.FromResult($"ER {exception.Message}");
        }
    }
}