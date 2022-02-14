using System;
using System.Linq;

namespace IISManager.Models
{
    public class Site
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int Port { get; set; }
        public string Url { get; set; }

        public Site()
        {
        }

        public Site(Microsoft.Web.Administration.Site site)
        {
            Id = site.Id;
            Name = site.Name;
            Path = site.Applications.First().VirtualDirectories.First().PhysicalPath;
            Url = site.Bindings.First().BindingInformation;
            Port = Convert.ToInt32(Url.Substring(Url.IndexOf(":") + 1).Replace(":", ""));
        }

        public Site(long id, string name, string path, string bindingInformation)
        {
            Id = id;
            Name = name;
            Path = path;
            Url = bindingInformation;
            Port = Convert.ToInt32(Url.Substring(Url.IndexOf(":") + 1).Replace(":", ""));
        }
    }
}