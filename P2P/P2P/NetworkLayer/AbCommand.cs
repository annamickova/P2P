using P2P.DataLayer;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class AbCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        if (args.Length != 1) return Task.FromResult("ER Špatný počet parametrů.");

        try
        {
            int accountId = CommandHelper.ParseAccountId(args[0]);
            
            var account = BankStorageSingleton.Instance.Dao.GetById(accountId);
            if (account == null) return Task.FromResult("ER Účet neexistuje.");
            
            string ip = args[0].Split("/")[1];
            if (ip != CommandHelper.MyIp)
            {
                Node node = new(ip);
                return node.SendRequestAsync($"AD {accountId}/{ip}")!;
            }

            return Task.FromResult($"AB {account.Balance}");
        }
        catch (Exception exception)
        {
            return Task.FromResult($"ER {exception.Message}");
        }
    }
}