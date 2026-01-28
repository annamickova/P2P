using P2P;
using P2P.NetworkLayer;
using P2P.Utils;

class Program
{
    static async Task Main(string[] args)
    {
        Logger.Configure();
        Server server = new Server();
        await server.StartAsync();

        /*
        CommandProcessor commandProcessor = new CommandProcessor();

        string input;
        do
        {
            input = Console.ReadLine();
            Console.WriteLine(await commandProcessor.Process(input));
        } while (input != "exit");
        */
    }
}