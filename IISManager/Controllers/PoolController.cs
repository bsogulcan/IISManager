using IISManager.Models;
using IISManager.Models.ResponseType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Web.Administration;

namespace IISManager.Controllers;

[ApiController]
[Route("iis/pool")]
public class PoolController(ILogger<PoolController> logger) : ControllerBase
{
    [HttpGet]
    public ActionResult<ResponseType<List<Pool>>> GetList()
    {
        var response = new ResponseType<List<Pool>>();

        try
        {
            using var serverMgr = new ServerManager();
            var pools = serverMgr.ApplicationPools
                .Select(pool =>
                    new Pool(pool))
                .ToList();

            response.Result = pools;
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

    [HttpGet("{name}")]
    public ActionResult<ResponseType<Pool>> Get(string name)
    {
        var response = new ResponseType<Pool>();

        try
        {
            using var serverMgr = new ServerManager();
            var pool = serverMgr.ApplicationPools.FirstOrDefault(x => x.Name == name);
            if (pool == null) throw new Exception("Pool not found! Name:" + name);

            response.Result = new Pool(pool);
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


    [HttpPost("{name}/stop")]
    public ActionResult<ResponseType<Pool>> Stop(string name)
    {
        var response = new ResponseType<Pool>();

        try
        {
            using var serverMgr = new ServerManager();
            var pool = serverMgr.ApplicationPools.FirstOrDefault(x => x.Name == name);
            if (pool == null) throw new Exception("Pool not found! Name:" + name);

            pool.Stop();
            serverMgr.CommitChanges();

            response.Result = new Pool(pool);
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

    [HttpPost("{name}/start")]
    public ActionResult<ResponseType<Pool>> Start(string name)
    {
        var response = new ResponseType<Pool>();

        try
        {
            using var serverMgr = new ServerManager();
            var pool = serverMgr.ApplicationPools.FirstOrDefault(x => x.Name == name);
            if (pool == null) throw new Exception("Pool not found! Name:" + name);

            pool.Start();
            serverMgr.CommitChanges();

            response.Result = new Pool(pool);
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

    [HttpPost("{name}/recycle")]
    public ActionResult<ResponseType<Pool>> Recycle(string name)
    {
        var response = new ResponseType<Pool>();

        try
        {
            using var serverMgr = new ServerManager();
            var pool = serverMgr.ApplicationPools.FirstOrDefault(x => x.Name == name);
            if (pool == null) throw new Exception("Pool not found! Name:" + name);

            pool.Recycle();
            serverMgr.CommitChanges();

            response.Result = new Pool(pool);
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

    [HttpDelete("{name}")]
    public ActionResult<ResponseType<Pool>> Delete(string name)
    {
        var response = new ResponseType<Pool>();

        try
        {
            using var serverMgr = new ServerManager();
            var pool = serverMgr.ApplicationPools.FirstOrDefault(x => x.Name == name);
            if (pool == null) throw new Exception("Pool not found! Name:" + name);

            pool.Delete();
            serverMgr.CommitChanges();

            response.Result = new Pool(pool);
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