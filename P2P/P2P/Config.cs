namespace P2P;
using Microsoft.Extensions.Configuration;

public static class Config
{
    public static string? ConnectionString { get; private set; }
    public static string? PrefferedStrategy { get; private set; }
    public static int ServerPort { get; private set; }
    public static int PortStart { get; private set; }
    public static int PortEnd { get; private set; }
    public static int ServerTimeout { get; private set; }
    public static int NodeTimeout { get; private set; }

public static void Load()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var storageSection = config.GetSection("Storage");

        PrefferedStrategy = storageSection["PrefferedStrategy"] ?? "memory";

        var serverSection = config.GetSection("Server");

        ServerPort = int.Parse(serverSection["Port"] ?? "65525");
        ServerTimeout = int.Parse(serverSection["Timeout"] ?? "10000");

        var peersSection = config.GetSection("Peers");

        PortStart = int.Parse(peersSection["PortStart"] ?? "65525");
        PortEnd = int.Parse(peersSection["PortEnd"] ?? "65535");
        NodeTimeout = int.Parse(peersSection["Timeout"] ?? "10000");

        var dbSection = config.GetSection("Database");

        string server = dbSection["Server"];
        string port = dbSection["Port"];
        string database = dbSection["Database"];
        string user = dbSection["User"];
        string password = dbSection["Password"];

        ConnectionString = $"server={server};port={port};database={database};user={user};password={password}";
    }
}