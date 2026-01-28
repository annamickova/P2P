using System.Net;
using System.Net.Sockets;

namespace P2P.Utils;

public static class CommandHelper
{
    public static string MyIp { get; private set; }

    static CommandHelper()
    {
        MyIp = GetLocalIpAddress();
        Console.WriteLine($"[Network] Detekována IP adresa uzlu: {MyIp}");
    }

    private static string GetLocalIpAddress()
    {
        try
        {
            Logger.Info($"Detected local IP: {MyIp}");

            var hostName = Dns.GetHostName();
            
            var hostEntry = Dns.GetHostEntry(hostName);

            foreach (var ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }
        catch (Exception exception)
        {
            Console.WriteLine($"[Network] Chyba při detekci IP: {exception.Message}");
            Logger.Warning("Invalid account format received.");
            return "127.0.0.1";
        }
    }

    public static int ParseAccountId(string rawInput)
    {
        var parts = rawInput.Split('/');
        if (parts.Length != 2) throw new Exception("Formát účtu musí být ČÍSLO/IP.");

        if (parts[1] != MyIp) throw new Exception($"Účet nepatří naší bance (IP nesedí. Očekáváno: {MyIp}, Přišlo: {parts[1]}).");
        
        if (!int.TryParse(parts[0], out int id)) throw new Exception("Číslo účtu není číslo.");
        
        if (id < 10000 || id > 99999) throw new Exception("Číslo účtu není v povoleném rozsahu 10000-99999.");
        
        return id;
    }

    public static long ParseAmount(string rawInput)
    {
        if (!long.TryParse(rawInput, out long amount)) 
            throw new Exception("Částka není platné číslo (long).");
            
        if (amount < 0) 
            throw new Exception("Částka nesmí být záporná.");

        return amount;
    }
}