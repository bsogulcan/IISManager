using IISManager.Managers.SiteManagers;
using IISManager.Models.Dtos;
using IISManager.Models.ResponseType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Web.Administration;
using Site = IISManager.Models.Site;

namespace IISManager.Controllers;

[ApiController]
[Route("iis/site")]
public class SiteController(
    ISiteManager siteManager,
    ILogger<PoolController> logger) : ControllerBase
{
    [HttpGet]
    public ActionResult<ResponseType<List<Site>>> GetAllSitesAsync()
    {
        var response = new ResponseType<List<Site>>();

        try
        {
            using var serverMgr = new ServerManager();
            var sites = serverMgr.Sites
                .Select(site =>
                    new Site(site))
                .ToList();

            response.Result = sites;
            return Ok(response);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Error = new ErrorInfo(e);
            logger.LogError(e, e.Message);
            return StatusCode(500, response);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<ResponseType<Site>> GetAsync(long id)
    {
        var response = new ResponseType<Site>();
        try
        {
            using var serverMgr = new ServerManager();
            var iisSite = serverMgr.Sites.FirstOrDefault(x => x.Id == id);
            if (iisSite == null) throw new Exception("Site not found! Id:" + id);

            response.Result = new Site(iisSite);
            return Ok(response);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Error = new ErrorInfo(e);
            logger.LogError(e, e.Message);
            return StatusCode(500, response);
        }
    }

    [HttpPost]
    public ActionResult<ResponseType<Site>> CreateAsync(IFormFile file, [FromForm] string name, [FromForm] int port,
        [FromForm] string bindingInformation)
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

            response.Result = siteManager.Create(createSiteInput);
            return Ok(response);
            
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Error = new ErrorInfo(e);
            logger.LogError(e, e.Message);
            return StatusCode(500, response);
        }
    }

    [HttpPut("{id}")]
    public ActionResult<ResponseType<Site>> UpdateAsync(long id, UpdateSiteDto input)
    {
        var response = new ResponseType<Site>();

        try
        {
            var updateSiteInput = new UpdateSiteInput()
            {
                Id = id,
                Name = input.Name,
                bindingInformation = input.BindingInformation
            };

            response.Result = siteManager.Update(updateSiteInput);
            return Ok(response);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Error = new ErrorInfo(e);
            logger.LogError(e, e.Message);
            return StatusCode(500, response);
        }
    }

    [HttpPost("{id}/deploy")]
    public ActionResult<ResponseType<Site>> Deploy(long id, IFormFile file)
    {
        var response = new ResponseType<Site>();

        try
        {
            var updateSiteInput = new DeploySiteInput()
            {
                Id = id,
                File = file
            };

            response.Result = siteManager.Deploy(updateSiteInput);
            return Ok(response);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Error = new ErrorInfo(e);
            logger.LogError(e, e.Message);
            return StatusCode(500, response);
        }
    }

    [HttpPost("{id}/stop")]
    public ActionResult<ResponseType<Site>> Stop(long id)
    {
        var response = new ResponseType<Site>();

        try
        {
            response.Result = siteManager.Stop(id);
            return Ok(response);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Error = new ErrorInfo(e);
            logger.LogError(e, e.Message);
            return StatusCode(500, response);
        }
    }

    [HttpPost("{id}/start")]
    public ActionResult<ResponseType<Site>> Start(long id)
    {
        var response = new ResponseType<Site>();

        try
        {
            response.Result = siteManager.Start(id);
            return Ok(response);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Error = new ErrorInfo(e);
            logger.LogError(e, e.Message);
            return StatusCode(500, response);
        }
    }
}