using System;
using System.Diagnostics;
using System.Linq;
using IISManager.Managers;
using Microsoft.Web.Administration;

namespace IISManager.Models
{
    public class Site
    {
        public long Id { get; set; }
        public string Name { get; set; }
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
            Path = site.Applications.First().VirtualDirectories.First().PhysicalPath;
            Url = site.Bindings.First().BindingInformation;
            Port = Convert.ToInt32(Url.Substring(Url.IndexOf(":") + 1).Replace(":", ""));
            State = SiteObjectStateConverter.GetString(site.State);
        }

        public Site(long id, string name, string path, string bindingInformation, ObjectState objectState)
        {
            Id = id;
            Name = name;
            Path = path;
            Url = bindingInformation;
            Port = Convert.ToInt32(Url.Substring(Url.IndexOf(":") + 1).Replace(":", ""));
            State = SiteObjectStateConverter.GetString(objectState);
        }
    }
}