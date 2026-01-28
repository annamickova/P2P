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

    public Task<string> Process(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return Task.FromResult("ER Empty command.");

        var parts = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var commandName = parts[0].ToUpper();
        
        var args = parts.Skip(1).ToArray();

        if (_commands.TryGetValue(commandName, out var command))
        {
            try
            {
                return command.ExecuteAsync(args);
            }
            catch (Exception exception)
            {
                return Task.FromResult($"ER Error while computing: {exception.Message}");
            }
        }

        return Task.FromResult("ER Unknown command.");
    }
}