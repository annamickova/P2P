using P2P.DataLayer;
using P2P.Utils;
using P2P.Monitoring;

namespace P2P.NetworkLayer;

public class AcCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        MonitoringState.SetLastCommand("AC");
        MonitoringState.IncrementCommands();

        var rnd = new Random();
        int newId;
        int attempts = 0;
        
        do 
        {
            newId = rnd.Next(10000, 100000);
            attempts++;
            if (attempts > 100)
            {
                string error = "Failed to generate unique account ID.";
                MonitoringState.IncrementErrors(error);
                Logger.Error(error);
                return Task.FromResult("ER Can't generate unique ID.");
            }
        } 
        while (BankStorageSingleton.Instance.Dao.GetById(newId) != null);

        var account = new BankAccount { AccountNumber = newId, Balance = 0 };
        
        if (BankStorageSingleton.Instance.Dao.Save(account))
        {
            Logger.Info($"Account created: {newId}");
            return Task.FromResult($"AC {newId}/{CommandHelper.MyIp}");
        }
        
        string saveError = "Failed while saving an account.";
        MonitoringState.IncrementErrors(saveError);
        Logger.Error(saveError);
        return Task.FromResult("ER Error while saving an account.");
    }
}