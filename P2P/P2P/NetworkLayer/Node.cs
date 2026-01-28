using System.Net.Sockets;
using System.Text;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class Node
{
    public string IP { get; set; }

    private StreamWriter _writer;
    private StreamReader _reader;

    public Node(string ip)
    {
        IP = ip;
    }

    public async Task<string?> SendRequestAsync(string command)
    {
        Logger.Debug($"Sending command to node {IP}: {command}");
            
        var result = await FindConnectionAsync(command);
        if (result == null)
        {
            Logger.Warning($"Failed to connect to node {IP}");
            throw new Exception();
        }

        return result;
    }

    private async Task<string?> FindConnectionAsync(string command)
    {
        TcpClient client = new();
        for (int port = Config.PortStart; port <= Config.PortEnd; port++)
        {
            try
            {
                Logger.Debug($"Trying {IP}:{port}");

                await client.ConnectAsync(IP, port);
                
                Logger.Info($"Connected to node {IP}:{port}");
                
                await using var stream = client.GetStream();
                
                stream.ReadTimeout = Config.NodeTimeout;
                stream.WriteTimeout = Config.NodeTimeout;

                _reader = new StreamReader(stream, Encoding.UTF8);
                _writer = new StreamWriter(stream, Encoding.UTF8) {AutoFlush =  true};
                
                await _writer.WriteLineAsync(command);
                return await _reader.ReadLineAsync();
            }
            catch (ArgumentNullException argumentNullException)
            {
                // The 'host' parameter is 'null'.
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                // The 'port' parameter is not between 'F:System.Net.IPEndPoint.MinPort' and 'F:System.Net.IPEndPoint.MaxPort'.
            }
            catch (SocketException socketException)
            {
                // An error occurred when accessing the socket.
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                // 'T:System.Net.Sockets.TcpClient' is closed.
            }
        }

        return null;
    }
}