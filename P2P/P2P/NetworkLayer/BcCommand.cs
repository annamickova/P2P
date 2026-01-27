using P2P.Utils;

namespace P2P.NetworkLayer;

public class BcCommand : ICommand
{
    public string Execute(string[] args)
    {
        return $"BC {CommandHelper.MyIp}";
    }
}