using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using IISManager.Managers.SiteManagers;
using IISManager.Models.Dtos;
using IISManager.Models.ResponseType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Web.Administration;
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
        public ResponseType<List<Site>> GetSitesAsync()
        {
            var response = new ResponseType<List<Site>>();

            try
            {
                using var serverMgr = new ServerManager();
                var sites = serverMgr.Sites
                    .Select(site =>
                        new Site(site.Id, site.Name,
                            site.Applications.First().VirtualDirectories.First().PhysicalPath,
                            site.Bindings.First().BindingInformation
                                .Substring(0, site.Bindings.First().BindingInformation.Length - 1), site.State))
                    .ToList();

                response.Result = sites;
                return response;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Error = new ErrorInfo(e);
                return response;
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("Get/{id}")]
        public ResponseType<Site> Get(long id)
        {
            var response = new ResponseType<Site>();
            try
            {
                using var serverMgr = new ServerManager();
                var iisSite = serverMgr.Sites.FirstOrDefault(x => x.Id == id);
                if (iisSite == null) throw new Exception("Site not found! Id:" + id);

                response.Result = new Site(iisSite);
                return response;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Error = new ErrorInfo(e);
                return response;
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("CreateFormData")]
        public ResponseType<Site> CreateFormData([FromForm] IFormFile file, [FromForm] string name, [FromForm] int port,
            [FromForm] string
                bindingInformation)
        {
            var response = new ResponseType<Site>();

            try
            {
                var createSiteInput = new CreateSiteInput()
                {
                    Name = name,
                    Port = port,
                    File = file,
                    BindingInformation = bindingInformation
                };

                response.Result = _siteManager.Create(createSiteInput);
                return response;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Error = new ErrorInfo(e);
                return response;
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("Update")]
        public ResponseType<Site> UpdateFormData([FromForm] long id, [FromForm] string name,
            [FromForm] string bindingInformation)
        {
            var response = new ResponseType<Site>();

            try
            {
                var updateSiteInput = new UpdateSiteInput()
                {
                    Id = id,
                    Name = name,
                    bindingInformation = bindingInformation
                };

                response.Result = _siteManager.Update(updateSiteInput);
                return response;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Error = new ErrorInfo(e);
                return response;
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("Deploy")]
        public ResponseType<Site> Deploy([FromForm] long id, [FromForm] IFormFile file)
        {
            var response = new ResponseType<Site>();

            try
            {
                var updateSiteInput = new DeploySiteInput()
                {
                    Id = id,
                    File = file
                };

                response.Result = _siteManager.Deploy(updateSiteInput);
                return response;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Error = new ErrorInfo(e);

                return response;
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("Stop")]
        public ResponseType<Site> Stop(StopSiteInput input)
        {
            var response = new ResponseType<Site>();

            try
            {
                response.Result = _siteManager.Stop(input);
                return response;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Error = new ErrorInfo(e);

                return response;
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("Start")]
        public ResponseType<Site> Start(StartSiteInput input)
        {
            var response = new ResponseType<Site>();

            try
            {
                response.Result = _siteManager.Start(input);
                return response;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Error = new ErrorInfo(e);

                return response;
            }
        }
    }
}