using System.Net.Sockets;
using System.Text;
using Org.BouncyCastle.Tls;

namespace P2P.NetworkLayer;

public class Node
{
    public int StartPort { get; set; }
    public int EndPort { get; set; }
    public string IP { get; set; }

    private StreamWriter _writer;
    private StreamReader _reader;

    public Node(string ip, int startPort, int endPort)
    {
        IP = ip;
        StartPort = startPort;
        EndPort = endPort;
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
        for (int port = StartPort; port <= EndPort; port++)
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