using P2P.DataLayer;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class AdCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        if (args.Length != 2) return Task.FromResult("ER Incorrect amount of parameters.");

        try
        {
            int accountId = CommandHelper.ParseAccountId(args[0].Split("/")[0]);

            long amount = CommandHelper.ParseAmount(args[1]);

            var dao = BankStorageSingleton.Instance.Dao;
            var account = dao.GetById(accountId);

            if (account == null) return Task.FromResult("ER Account doesn't exist.");

            string ip = args[0].Split("/")[1];
            if (ip != CommandHelper.MyIp)
            {
                Node node = new(ip);
                return node.SendRequestAsync($"AD {accountId}/{ip} {amount}")!;
            }

            account.Balance += amount;
            
            if (dao.Update(account))
            {
                return Task.FromResult("AD");
            }
            return Task.FromResult("ER Error while depositing.");
        }
        catch (Exception exception)
        {
            return Task.FromResult($"ER {exception.Message}");
        }
    }
}