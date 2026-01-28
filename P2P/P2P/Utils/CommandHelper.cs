using System.Net;
using System.Net.Sockets;

namespace P2P.Utils;

public static class CommandHelper
{
    public static string MyIp => Config.ServerIP;

    private static string GetLocalIpAddress()
    {
        try
        {
            Logger.Info($"Detected local IP: {MyIp}");

            try
            {
                return IPAddress.Parse(Config.ServerIP).ToString();
            }
            catch (Exception)
            {
                Logger.Error("Invalid IP address, using 127.0.0.1");
            }

            return "127.0.0.1";
        }
        catch (Exception)
        {
            Logger.Warning("Invalid IP format received.");
            return "127.0.0.1";
        }
    }

    public static int ParseAccountId(string rawInput)
    {
        var parts = rawInput.Split('/');
        if (parts.Length != 2) throw new Exception("The format of account must be ID/IP.");
        
        if (!int.TryParse(parts[0], out int id)) throw new Exception("Bank account ID isn't a number.");
        
        if (id < 10000 || id > 99999) throw new Exception("Bank account ID isn't in range 10000-99999.");
        
        return id;
    }

    public static long ParseAmount(string rawInput)
    {
        if (!long.TryParse(rawInput, out long amount)) 
            throw new Exception("The amount is not a number.");
            
        if (amount < 0) 
            throw new Exception("The amount must not be negative.");

        return amount;
    }
}