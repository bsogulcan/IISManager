using IISManager.Managers;
using Microsoft.Web.Administration;

namespace IISManager.Models;

public class Site
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string AppPoolName { get; set; }
    public string Path { get; set; }
    public int Port { get; set; }
    public string Url { get; set; }
    public string State { get; set; }

    public Site()
    {
    }

    public Site(Microsoft.Web.Administration.Site site)
    {
        Id = site.Id;
        Name = site.Name;
        AppPoolName = site.ApplicationDefaults?.ApplicationPoolName;
        Path = Environment.ExpandEnvironmentVariables(site.Applications.First().VirtualDirectories.First()
            .PhysicalPath);
        Url = site.Bindings.First().BindingInformation;
        // Port = Convert.ToInt32(Url.Substring(Url.IndexOf(":") + 1).Replace(":", ""));
        
        if (int.TryParse(Url.Split(':')[1], out var port))
        {
            Port = port;
        }
        else
        {
            Port = -1;
        }
        
        try
        {
            State = StateConverter.GetString(site.State);
        }
        catch
        {
            State = "Unknown";
        }
    }
}