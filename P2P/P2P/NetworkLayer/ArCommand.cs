using P2P.DataLayer;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class ArCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        if (args.Length != 1) return Task.FromResult("ER Špatný počet parametrů.");

        try
        {
            int accountId = CommandHelper.ParseAccountId(args[0]);
            var dao = BankStorageSingleton.Instance.Dao;
            var account = dao.GetById(accountId);

            if (account == null) return Task.FromResult("ER Účet neexistuje.");

            string ip = args[0].Split("/")[1];
            if (ip != CommandHelper.MyIp)
            {
                Node node = new(ip);
                return node.SendRequestAsync($"AD {accountId}/{ip}")!;
            }
            
            if (account.Balance != 0) return Task.FromResult("ER Nelze smazat bankovní účet na kterém jsou finance.");

            if (dao.Delete(accountId)) return Task.FromResult("AR");
            
            return Task.FromResult("ER Chyba databáze při mazání.");
        }
        catch (Exception exception)
        {
            return Task.FromResult($"ER {exception.Message}");
        }
    }
}