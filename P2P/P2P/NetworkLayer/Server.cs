using System.Net.Sockets;
using System.Text;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class Server
{
    private TcpListener _listener;
    private CancellationTokenSource _tokenSource;

    public Server()
    {
        _listener = new(System.Net.IPAddress.Any, Config.ServerPort);
        _tokenSource = new();
    }

    public async Task StartAsync()
    {
        Logger.Info($"Server started on port: {Config.ServerPort}");

        _listener.Start();

        CommandProcessor processor = new();
        
        while (!_tokenSource.IsCancellationRequested)
        {
            TcpClient client = await _listener.AcceptTcpClientAsync();
            Logger.Info("Client connected.");

            await Task.Run(() =>
                {
                    using var stream = client.GetStream();
                    stream.ReadTimeout = Config.ServerTimeout;
                    stream.WriteTimeout = Config.ServerTimeout;
                    using var streamReader = new StreamReader(stream, Encoding.UTF8);
                    using var streamWriter = new StreamWriter(stream, Encoding.UTF8);

                    string input;
                    do
                    {
                        input = streamReader.ReadLine()!;
                        
                        Logger.Debug($"Received command: {input}");

                        streamWriter.WriteLine(processor.Process(input));
                        
                        Logger.Debug("Response sent to client.");

                    } while (input != "exit");
                },
                _tokenSource.Token);
        }
    }
}