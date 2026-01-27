using P2P.DataLayer;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class AcCommand : ICommand
{
    public string Execute(string[] args)
    {
        // AC nemá argumenty
        
        // 1. Vygenerovat ID (10000 - 99999)
        // Ve skutečné aplikaci bys měl v DAO metodu GetNextId(), 
        // tady to pro zjednodušení střelíme od boku nebo náhodně s kontrolou.
        var rnd = new Random();
        int newId;
        int attempts = 0;
        
        do 
        {
            newId = rnd.Next(10000, 100000);
            attempts++;
            if (attempts > 100) return "ER Nelze vygenerovat unikátní ID.";
        } 
        while (BankStorageSingleton.Instance.Dao.GetById(newId) != null);

        var account = new BankAccount { AccountNumber = newId, Balance = 0 };
        
        if (BankStorageSingleton.Instance.Dao.Save(account))
        {
            return $"AC {newId}/{CommandHelper.MyIp}";
        }
        
        return "ER Chyba při ukládání účtu.";
    }
}