namespace Cinema.Commands;

public interface ICommand
{
    public void Execute();
    string Description { get; }
}