using P2P.DataLayer;

namespace P2P.NetworkLayer;

public class BnCommand : ICommand
{
    public string Execute(string[] args)
    {
        try
        {
            var allAccounts = BankStorageSingleton.Instance.Dao.GetAll();
            
            return $"BN {allAccounts.Count}";
        }
        catch (Exception exception)
        {
            return $"ER Chyba při zjišťování počtu klientů: {exception.Message}";
        }
    }
}