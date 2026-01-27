namespace P2P.NetworkLayer;

public interface ICommand
{
    string Execute(string[] args);
}