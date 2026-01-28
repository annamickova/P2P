using P2P.DataLayer;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class AdCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        if (args.Length != 2) return Task.FromResult("ER Špatný počet parametrů.");

        try
        {
            int accountId = CommandHelper.ParseAccountId(args[0].Split("/")[0]);
            
            if (!long.TryParse(args[1], out long amount) || amount <= 0)
                return Task.FromResult("ER Částka musí být kladné číslo.");

            var dao = BankStorageSingleton.Instance.Dao;
            var account = dao.GetById(accountId);

            if (account == null) return Task.FromResult("ER Účet neexistuje.");

            string ip = args[0].Split("/")[1];
            if (ip != CommandHelper.MyIp)
            {
                Node node = new(ip, 65525, 6535);
                return node.SendRequestAsync($"AD {accountId}/{ip} {amount}")!;
            }

            account.Balance += amount;
            
            if (dao.Update(account))
            {
                return Task.FromResult("AD");
            }
            return Task.FromResult("ER Chyba databáze při vkladu.");
        }
        catch (Exception exception)
        {
            return Task.FromResult($"ER {exception.Message}");
        }
    }
}