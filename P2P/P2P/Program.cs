using Microsoft.Extensions.Configuration;
using P2P.DataLayer;
using System.Net;
using P2P.NetworkLayer;
using P2P.Utils;

class Program
{
    private static readonly CommandProcessor _localProcessor = new CommandProcessor();
    private static Node _node;

    static async Task Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== P2P BANK NODE STARTUP ===");
        Console.ResetColor();

        // 1. NAČTENÍ KONFIGURACE (appsettings.json)
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        IConfiguration config = builder.Build();

        string connString = config.GetConnectionString("DefaultConnection");
        string strategy = config["AppSettings:Strategy"] ?? "memory";

        // 2. INICIALIZACE ÚLOŽIŠTĚ (DB / FILE / MEMORY)
        Console.WriteLine($"[Init] Inicializuji úložiště: {strategy.ToUpper()}...");
        BankStorageSingleton.Instance.Initialize(strategy, connString);

        // 3. NASTAVENÍ SÍTĚ
        // Zjistíme naši IP (díky tvému CommandHelperu)
        string myIp = CommandHelper.MyIp;
        Console.WriteLine($"[Init] Moje IP adresa: {myIp}");

        // Zeptáme se na port (abychom mohli na jednom PC spustit více instancí)
        Console.Write("[Init] Zadej port pro tento uzel (např. 5000): ");
        if (!int.TryParse(Console.ReadLine(), out int port)) port = 5000;

        // 4. SPUŠTĚNÍ P2P SERVERU
        _node = new Node(port);
        // Spustíme server na pozadí (neblokujeme tím psaní příkazů)
        _ = _node.StartServerAsync();

        // 5. INTERAKTIVNÍ SMYČKA
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n=== UZEL BĚŽÍ A ČEKÁ NA PŘÍKAZY ===");
        Console.ResetColor();
        Console.WriteLine("Možnosti:");
        Console.WriteLine(" - 'connect <ip> <port>' : Připojí se k jinému uzlu.");
        Console.WriteLine(" - 'BC', 'AC', 'BA'...   : Vykoná bankovní příkaz lokálně.");
        Console.WriteLine(" - 'exit'                : Ukončí program.");
        Console.WriteLine("---------------------------------------------------\n");

        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) continue;

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) 
            {
                break;
            }
            else if (input.StartsWith("connect", StringComparison.OrdinalIgnoreCase))
            {
                // PŘÍKAZ PRO PŘIPOJENÍ: connect 127.0.0.1 5001
                var parts = input.Split(' ');
                if (parts.Length == 3 && int.TryParse(parts[2], out int targetPort))
                {
                    await _node.ConnectToPeerAsync(parts[1], targetPort);
                }
                else
                {
                    Console.WriteLine("Chyba: Použij formát 'connect <ip> <port>'");
                }
            }
            else
            {
                // JAKÝKOLIV JINÝ TEXT POVAŽUJEME ZA BANKOVNÍ PŘÍKAZ (Testujeme logiku lokálně)
                // Simulujeme, jako by příkaz přišel ze sítě, ale vypíšeme si výsledek hned.
                string response = _localProcessor.Process(input);
                
                // Obarvíme výstup podle toho, jestli je to chyba (ER) nebo úspěch
                if (response.StartsWith("ER")) Console.ForegroundColor = ConsoleColor.Red;
                else Console.ForegroundColor = ConsoleColor.Yellow;
                
                Console.WriteLine(response);
                Console.ResetColor();
            }
        }
    }
}