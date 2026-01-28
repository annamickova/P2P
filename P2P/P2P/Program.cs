using P2P;
using P2P.NetworkLayer;

class Program
{
    static async Task Main(string[] args)
    {
        Server server = new Server();
        await server.StartAsync();
    }
}