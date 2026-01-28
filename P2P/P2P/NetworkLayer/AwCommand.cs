using P2P.DataLayer;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class AwCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        if (args.Length != 2) return Task.FromResult("ER Incorrect amount of nodes.");

        try
        {
            int accountId = CommandHelper.ParseAccountId(args[0]);
            if (!long.TryParse(args[1], out long amount) || amount <= 0)
                return Task.FromResult("ER The amount of money must be a positive whole number.");

            var dao = BankStorageSingleton.Instance.Dao;
            var account = dao.GetById(accountId);

            if (account == null) return Task.FromResult("ER Účet neexistuje.");
            
            string ip = args[0].Split("/")[1];
            if (ip != CommandHelper.MyIp)
            {
                Node node = new(ip);
                return node.SendRequestAsync($"AD {accountId}/{ip} {amount}")!;
            }

            if (account.Balance < amount) return Task.FromResult("ER Not enough money saved.");

            account.Balance -= amount;
            
            if (dao.Update(account)) return Task.FromResult("AW");
            
            return Task.FromResult("ER Error while withdrawing.");
        }
        catch (Exception exception)
        {
            return Task.FromResult($"ER {exception.Message}");
        }
    }
}