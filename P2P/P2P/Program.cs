using P2P;
using P2P.NetworkLayer;
using P2P.Utils;

class Program
{
    static async Task Main(string[] args)
    {
        Config.Load();
        Logger.Configure();
        Server server = new Server();
        await server.StartAsync();
    }
}