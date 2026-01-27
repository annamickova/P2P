using P2P.DataLayer;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class AdCommand : ICommand
{
    public string Execute(string[] args)
    {
        if (args.Length != 2) return "ER Špatný počet parametrů.";

        try
        {
            int accountId = CommandHelper.ParseAccountId(args[0]);
            
            if (!long.TryParse(args[1], out long amount) || amount <= 0)
                return "ER Částka musí být kladné číslo.";

            var dao = BankStorageSingleton.Instance.Dao;
            var account = dao.GetById(accountId);

            if (account == null) return "ER Účet neexistuje.";

            account.Balance += amount;
            
            if (dao.Update(account))
            {
                return "AD";
            }
            return "ER Chyba databáze při vkladu.";
        }
        catch (Exception exception)
        {
            return $"ER {exception.Message}";
        }
    }
}