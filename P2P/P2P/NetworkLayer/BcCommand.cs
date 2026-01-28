using P2P.Utils;

namespace P2P.NetworkLayer;

public class BcCommand : ICommand
{
    public Task<string> ExecuteAsync(string[] args)
    {
        Logger.Debug("Bank code requested.");
        return Task.FromResult($"BC {CommandHelper.MyIp}");
    }
}