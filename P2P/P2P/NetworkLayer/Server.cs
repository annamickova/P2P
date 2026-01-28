using System.Net;
using System.Net.Sockets;
using System.Text;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class Server
{
    private TcpListener _listener;
    private CancellationTokenSource _tokenSource;
    private CommandProcessor _processor;

    public Server()
    {
        _listener = new(IPAddress.Parse(Config.ServerIP), Config.ServerPort);
        _tokenSource = new();
        _processor = new ();
    }

    public async Task StartAsync()
    {
        _listener.Start();
        
        Logger.WriteToConsole(LogLevel.INFO, "Server is running");
        
        while (!_tokenSource.IsCancellationRequested)
        {
            TcpClient client = await _listener.AcceptTcpClientAsync();
            
            Logger.WriteToConsole(LogLevel.INFO, "New client connected.");

            HandleClient(client);
        }
    }

    private async Task HandleClient(TcpClient client)
    {
        await using var stream = client.GetStream();
        stream.ReadTimeout = Config.ServerTimeout;
        stream.WriteTimeout = Config.ServerTimeout;
        using var streamReader = new StreamReader(stream, Encoding.UTF8);
        await using var streamWriter = new StreamWriter(stream, Encoding.UTF8) {AutoFlush = true};

        string? input;
        do
        {
            input = await streamReader.ReadLineAsync();

            await streamWriter.WriteLineAsync(await _processor.Process(input));
        } while (input != "exit");
    }
}