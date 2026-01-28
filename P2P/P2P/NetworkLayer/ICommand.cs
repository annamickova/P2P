namespace P2P.NetworkLayer;

public interface ICommand
{
    Task<string> ExecuteAsync(string[] args);
}