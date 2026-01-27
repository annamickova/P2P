namespace P2P.NetworkLayer;

public class CommandProcessor
{
    private readonly Dictionary<string, ICommand> _commands;

    public CommandProcessor()
    {
        _commands = new Dictionary<string, ICommand>
        {
            { "BC", new BcCommand() },
            { "AC", new AcCommand() },
            { "AD", new AdCommand() },
            { "AW", new AwCommand() },
            { "AB", new AbCommand() },
            { "AR", new ArCommand() },
            { "BA", new BaCommand() },
            { "BN", new BnCommand() }
        };
    }

    public string Process(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return "ER Prázdný příkaz.";

        var parts = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var commandName = parts[0].ToUpper();
        
        // Získáme argumenty (všechno za prvním slovem)
        var args = parts.Skip(1).ToArray();

        if (_commands.TryGetValue(commandName, out var command))
        {
            try
            {
                return command.Execute(args);
            }
            catch (Exception ex)
            {
                // Jakákoliv neošetřená chyba v logice příkazu vrátí ER
                return $"ER Chyba při zpracování: {ex.Message}";
            }
        }

        return "ER Neznámý příkaz.";
    }
}