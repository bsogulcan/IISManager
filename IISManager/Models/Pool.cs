using IISManager.Managers;
using Microsoft.Web.Administration;

namespace IISManager.Models;

public class Pool
{
    public string Name { get; set; }
    public string State { get; set; }

    public Pool(string name, string state)
    {
        Name = name;
        State = state;
    }

    public Pool(ApplicationPool pool)
    {
        Name = pool.Name;
        State = StateConverter.GetString(pool.State);
    }
}