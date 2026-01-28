using P2P.Utils;

namespace P2P.DataLayer;

using System;
using MySql.Data.MySqlClient;

public sealed class BankStorageSingleton
{
    private static readonly Lazy<BankStorageSingleton> Lazy = new(() => new BankStorageSingleton(), true);

    public static BankStorageSingleton Instance => Lazy.Value;

    public IGenericDao<BankAccount> Dao { get; private set; }

    private BankStorageSingleton()
    {
        Config.Load();
        Dao = Initialize(Config.PrefferedStrategy, Config.ConnectionString); 
    }

    public IGenericDao<BankAccount> Initialize(string preferredStrategy, string connectionString = "")
    {
        bool success = false;

        if (preferredStrategy.Equals("mysql", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var dbDao = new DbBankAccountDAO(() => new MySqlConnection(connectionString));
                
                dbDao.Initialize(); 
                
                Dao = dbDao;
                success = true;
                Logger.Info($"Storage initialized using: {preferredStrategy}");
            }
            catch (Exception)
            {
                preferredStrategy = "file"; 
            }
        }

        if (!success && preferredStrategy.Equals("file", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var fileDao = new FileBankAccountDao();
                
                fileDao.Initialize(); 
                
                Dao = fileDao;
                success = true;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        if (!success)
        {
            Dao = new MemoryBankAccountDao();
            Dao.Initialize();
        }
        
        return Dao;
    }
}