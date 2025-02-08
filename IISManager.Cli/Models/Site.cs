namespace IISManager.Cli.Models;

public class Site
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public int Port { get; set; }
    public string Url { get; set; }
    public string State { get; set; }
}