using P2P.DataLayer;

namespace P2P.NetworkLayer;

public class BaCommand : ICommand
{
    public string Execute(string[] args)
    {
        try
        {
            var allAccounts = BankStorageSingleton.Instance.Dao.GetAll();
            
            long totalAmount = allAccounts.Sum(bankAccount => bankAccount.Balance);

            return $"BA {totalAmount}";
        }
        catch (Exception exception)
        {
            return $"ER Chyba při výpočtu celkové sumy: {exception.Message}";
        }
    }
}