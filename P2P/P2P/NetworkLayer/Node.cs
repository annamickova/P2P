using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using P2P.NetworkLayer;

public class Node
{
    private readonly int _port;
    private TcpListener _listener;
    
    // Seznam připojených uzlů (IP:Port -> StreamWriter pro odesílání dat)
    // ConcurrentDictionary je nutný, protože s ním pracuje více vláken najednou.
    private readonly ConcurrentDictionary<string, StreamWriter> _peers = new();

    // Mozek banky - zde se rozhoduje, co který příkaz (BC, AC, AD...) udělá
    private readonly CommandProcessor _processor = new CommandProcessor();

    public Node(int port)
    {
        _port = port;
    }
    public async Task StartServerAsync()
    {
        _listener = new TcpListener(IPAddress.Any, _port);
        _listener.Start();
        Console.WriteLine($"[P2P Server] Běží na portu {_port}. Čekám na spojení...");

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            
            _ = HandleClientAsync(client);
        }
    }
    
    public async Task ConnectToPeerAsync(string ip, int port)
    {
        try
        {
            var client = new TcpClient();
            Console.WriteLine($"[P2P Client] Připojuji se k {ip}:{port}...");
            
            await client.ConnectAsync(ip, port);
            Console.WriteLine($"[P2P Client] -> Úspěšně připojeno k {ip}:{port}!");

            _ = HandleClientAsync(client);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[P2P Client] Chyba připojení k {ip}:{port}: {ex.Message}");
        }
    }
    
    private async Task HandleClientAsync(TcpClient client)
    {
        string peerEndpoint = client.Client.RemoteEndPoint?.ToString() ?? "Unknown";

        using var networkStream = client.GetStream();
        using var reader = new StreamReader(networkStream);
        
        using var writer = new StreamWriter(networkStream);
        writer.AutoFlush = true;

        _peers.TryAdd(peerEndpoint, writer);

        try
        {
            string? message;
            while ((message = await reader.ReadLineAsync()) != null)
            {
                Console.WriteLine($"[P2P] Příkaz od {peerEndpoint}: {message}");
                string response = _processor.Process(message);

                await writer.WriteLineAsync(response);
                
                Console.WriteLine($"[P2P] Odpověď pro {peerEndpoint}: {response}");
            }
        }
        catch (IOException)
        {
            Console.WriteLine($"[P2P] Uzel {peerEndpoint} se odpojil.");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"[P2P] Chyba komunikace s {peerEndpoint}: {exception.Message}");
        }
        finally
        {
            _peers.TryRemove(peerEndpoint, out _);
            client.Close();
        }
    }
    public async Task BroadcastAsync(string message)
    {
        Console.WriteLine($"[P2P Broadcast] Rozesílám všem: {message}");
        foreach (var peer in _peers)
        {
            try
            {
                await peer.Value.WriteLineAsync(message);
            }
            catch
            {
                // ignored
            }
        }
    }
}