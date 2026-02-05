using P2P;
using P2P.Monitoring;
using P2P.NetworkLayer;
using P2P.Utils;

class Program
{
    static async Task Main(string[] args)
    {
        var monitoringServer = new MonitoringServer();
        _ = monitoringServer.StartAsync();

        Config.Load();
        Logger.Configure();
        Server server = new Server();
        await server.StartAsync();
    }
}