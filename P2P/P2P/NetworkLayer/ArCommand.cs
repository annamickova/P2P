using P2P.DataLayer;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class ArCommand : ICommand
{
    public string Execute(string[] args)
    {
        if (args.Length != 1) return "ER Špatný počet parametrů.";

        try
        {
            int accountId = CommandHelper.ParseAccountId(args[0]);
            var dao = BankStorageSingleton.Instance.Dao;
            var account = dao.GetById(accountId);

            if (account == null) return "ER Účet neexistuje.";
            
            if (account.Balance != 0) return "ER Nelze smazat bankovní účet na kterém jsou finance.";

            if (dao.Delete(accountId)) return "AR";
            
            return "ER Chyba databáze při mazání.";
        }
        catch (Exception exception)
        {
            return $"ER {exception.Message}";
        }
    }
}