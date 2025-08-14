namespace Cinema.Commands;

public interface ICommand
{
    public bool? Execute();
    string Name { get; }
    string Description { get; }
}