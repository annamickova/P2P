using P2P.DataLayer;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class AwCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        if (args.Length != 2) return Task.FromResult("ER Incorrect amount of parameters.");

        try
        {
            int accountId = CommandHelper.ParseAccountId(args[0]);

            long amount = CommandHelper.ParseAmount(args[1]);
            
            Logger.Info($"Withdraw request: account {accountId}, amount {amount}");
            var dao = BankStorageSingleton.Instance.Dao;
            var account = dao.GetById(accountId);

            if (account == null)
            {
                Logger.Error($"Withdraw request: account {accountId} not found");
                return Task.FromResult("ER Účet neexistuje.");
            }
            
            string ip = args[0].Split("/")[1];
            if (ip != CommandHelper.MyIp)
            {
                Node node = new(ip);
                return node.SendRequestAsync($"AW {accountId}/{ip} {amount}")!;
            }

            if (account.Balance < amount)
            {
                Logger.Warning($"Withdraw request: account {accountId} not balanced.");
                return Task.FromResult("ER Not enough money saved.");
            }

            account.Balance -= amount;
            if (dao.Update(account))
            {
                Logger.Info($"Withdraw successful: account {accountId}, new balance {account.Balance}");
                return Task.FromResult("AW");
            }
            
            return Task.FromResult("ER Error while withdrawing.");
        }
        catch (Exception exception)
        {
            return Task.FromResult($"ER {exception.Message}");
        }
    }
}