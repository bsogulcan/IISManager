namespace IISManager.Cli.Models;

public class Pool
{
    public string Name { get; set; }
    public string State { get; set; }

    public Pool(string name, string state)
    {
        Name = name;
        State = state;
    }
}