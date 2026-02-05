using P2P.Utils;
using P2P.Monitoring;

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
        Logger.Debug($"Processing command: {message}");

        if (string.IsNullOrWhiteSpace(message))
        {
            Logger.Warning("Empty command received.");
            return Task.FromResult("ER Empty command.");
        }

        var parts = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var commandName = parts[0].ToUpper();
        MonitoringState.SetLastCommand(commandName);
        MonitoringState.IncrementCommands();
        
        var args = parts.Skip(1).ToArray();

        if (_commands.TryGetValue(commandName, out var command))
        {
            try
            {
                return command.ExecuteAsync(args);
            }
            catch (Exception exception)
            {
                MonitoringState.IncrementErrors(exception.Message);
                Logger.Error($"Processing failed: {exception.Message}");
                return Task.FromResult($"ER Processing failed: {exception.Message}");
            }
        }
        MonitoringState.IncrementErrors($"Unknown command: {commandName}");
        Logger.Warning($"Unknown command: {commandName}");
        return Task.FromResult("ER Unknown command.");
    }
}