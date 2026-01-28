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
        Dao = Initialize("mysql", Config.ConnectionString); 
    }

    public IGenericDao<BankAccount> Initialize(string preferredStrategy, string connectionString = "")
    {
        bool success = false;

        if (preferredStrategy.Equals("mysql", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                Console.WriteLine("[Storage] Testing connection to MySQL...");
                var dbDao = new DbBankAccountDAO(() => new MySqlConnection(connectionString));
                
                dbDao.Initialize(); 
                
                Dao = dbDao;
                success = true;
                Console.WriteLine("[Storage] -> Successfully connected to MySQL.");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"[Storage] WARNING: MySQL failed ({exception.Message}).");
                Console.WriteLine("[Storage] -> Switching to backup solution (JSON).");
                
                preferredStrategy = "file"; 
            }
        }

        if (!success && preferredStrategy.Equals("file", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                Console.WriteLine("[Storage] Attempting to initialize file storage...");
                var fileDao = new FileBankAccountDao();
                
                fileDao.Initialize(); 
                
                Dao = fileDao;
                success = true;
                Console.WriteLine("[Storage] -> Successfully set file storage.");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"[Storage] WARNING: File storage failed ({exception.Message}).");
                Console.WriteLine("[Storage] -> Switching to last resort (Memory).");
            }
        }

        if (!success)
        {
            Console.WriteLine("[Storage] Initializing memory storage (RAM only)...");
            Dao = new MemoryBankAccountDao();
            Dao.Initialize();
            Console.WriteLine("[Storage] -> Running in memory. Data will disappear after shut down.");
        }
        
        return Dao;
    }
}