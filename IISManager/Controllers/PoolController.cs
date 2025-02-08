using IISManager.Models;
using IISManager.Models.ResponseType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Web.Administration;

namespace IISManager.Controllers;

[ApiController]
[Route("iis/pool")]
public class PoolController : ControllerBase
{
    [HttpGet]
    public Task<ResponseType<List<Pool>>> GetList()
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
            return Task.FromResult(response);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Error = new ErrorInfo(e);
            return Task.FromResult(response);
        }
    }

    [HttpGet("{name}")]
    public Task<ResponseType<Pool>> Get(string name)
    {
        var response = new ResponseType<Pool>();

        try
        {
            using var serverMgr = new ServerManager();
            var pool = serverMgr.ApplicationPools.FirstOrDefault(x => x.Name == name);
            if (pool == null) throw new Exception("Pool not found! Name:" + name);

            response.Result = new Pool(pool);
            return Task.FromResult(response);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Error = new ErrorInfo(e);
            return Task.FromResult(response);
        }
    }


    [HttpPost("{name}/stop")]
    public Task<ResponseType<Pool>> Stop(string name)
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
            return Task.FromResult(response);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Error = new ErrorInfo(e);
            return Task.FromResult(response);
        }
    }

    [HttpPost("{name}/start")]
    public Task<ResponseType<Pool>> Start(string name)
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
            return Task.FromResult(response);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Error = new ErrorInfo(e);
            return Task.FromResult(response);
        }
    }

    [HttpPost("{name}/recycle")]
    public Task<ResponseType<Pool>> Recycle(string name)
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
            return Task.FromResult(response);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Error = new ErrorInfo(e);
            return Task.FromResult(response);
        }
    }

    [HttpDelete("{name}")]
    public Task<ResponseType<Pool>> Delete(string name)
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
            return Task.FromResult(response);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Error = new ErrorInfo(e);
            return Task.FromResult(response);
        }
    }
}