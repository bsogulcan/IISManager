using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using IISManager.Managers;
using IISManager.Managers.SiteManagers;
using IISManager.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Web.Administration;
using Newtonsoft.Json;
using Site = IISManager.Models.Site;

namespace IISManager.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
    public class IisController : ControllerBase
    {
        private readonly ISiteManager _siteManager;

        public IisController(ISiteManager siteManager)
        {
            _siteManager = siteManager;
        }

        [Microsoft.AspNetCore.Mvc.Route("GetAll")]
        public List<Site> GetSitesAsync()
        {
            using var serverMgr = new ServerManager();
            var sites = serverMgr.Sites
                .Select(site =>
                    new Site(site.Id, site.Name,
                        site.Applications.First().VirtualDirectories.First().PhysicalPath,
                        site.Bindings.First().BindingInformation))
                .ToList();

            return sites;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("Get/{id}")]
        public Site GetAll(long id)
        {
            using var serverMgr = new ServerManager();
            var iisSite = serverMgr.Sites.FirstOrDefault(x => x.Id == id);
            if (iisSite == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            var site = new Site(iisSite);
            return site;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("CreateFormData")]
        public Site CreateFormData([FromForm] IFormFile file, [FromForm] string name, [FromForm] int port)
        {
            var createSiteInput = new CreateSiteInput()
            {
                Name = name,
                Port = port,
                File = file
            };

            var site = _siteManager.Create(createSiteInput);
            return site;
        }
    }
}