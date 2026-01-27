using P2P.DataLayer;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class AbCommand : ICommand
{
    public string Execute(string[] args)
    {
        if (args.Length != 1) return "ER Špatný počet parametrů.";

        try
        {
            int accountId = CommandHelper.ParseAccountId(args[0]);
            
            var account = BankStorageSingleton.Instance.Dao.GetById(accountId);
            if (account == null) return "ER Účet neexistuje.";

            return $"AB {account.Balance}";
        }
        catch (Exception exception)
        {
            return $"ER {exception.Message}";
        }
    }
}