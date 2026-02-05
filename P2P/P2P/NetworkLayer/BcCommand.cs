using P2P.Monitoring;
using P2P.Utils;

namespace P2P.NetworkLayer;

public class BcCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        MonitoringState.SetLastCommand("BC");
        MonitoringState.IncrementCommands();
        Logger.Debug("Bank code requested.");
        return Task.FromResult($"BC {CommandHelper.MyIp}");
    }
}