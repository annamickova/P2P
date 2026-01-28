using System.Net.Sockets;
using System.Text;

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
        TcpClient? client = await FindConnectionAsync();

        if (client is null)
        {
            throw new Exception();
        }
        
        await _writer.WriteLineAsync(command);
        return await _reader.ReadLineAsync();
    }

    private async Task<TcpClient?> FindConnectionAsync()
    {
        TcpClient client = new();
        for (int port = Config.PortStart; port <= Config.PortEnd; port++)
        {
            try
            {
                await client.ConnectAsync(IP, port);
                
                using var stream = client.GetStream();
                int timeout = 5000;
                stream.ReadTimeout = timeout;
                stream.WriteTimeout = timeout;

                _reader = new StreamReader(stream, Encoding.UTF8);
                _writer = new StreamWriter(stream, Encoding.UTF8);
        
                _writer.WriteLine("BC");

                var peerResponse = _reader.ReadLine();
                if (peerResponse is null || !peerResponse.StartsWith("BC"))
                {
                    continue;
                }
                
                return client;
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