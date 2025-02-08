using System.IO.Compression;
using System.Text.Json;
using IISManager.Cli.Models;
using IISManager.Cli.Models.Dtos;
using IISManager.Cli.Models.ResponseType;
using RestSharp;

namespace IISManager.Cli.Manager
{
    public class IISManager
    {
        private readonly string _host;
        private readonly int _port;

        public IISManager(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public async Task<List<Site>> GetList()
        {
            var response = await HttpManager.Request(_host + ":" + _port, "/iis/site", Method.Get, null);
            if (!response.IsSuccessful) throw new Exception(response.ErrorMessage);

            var result = JsonSerializer.Deserialize<ResponseType<List<Site>>>(response.Content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            if (result is { IsSuccess: true })
            {
                var table = new TablePrinter("Id", "Name", "PoolName", "Path", "Port", "Url", "State");
                foreach (var site in result.Result)
                {
                    table.AddRow(site.Id, site.Name, site.AppPoolName, site.Path, site.Port, site.Url, site.State);
                }

                table.Print();


                return result.Result;
            }

            throw new Exception(response.ErrorMessage);
        }

        public async Task<Site> Get(int id)
        {
            try
            {
                var response = await HttpManager.Request(_host + ":" + _port, "/iis/site/" + id, Method.Get, null);
                if (!response.IsSuccessful) throw new Exception(response.ErrorMessage);

                var result = JsonSerializer.Deserialize<ResponseType<Site>>(response.Content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                if (result is { IsSuccess: true })
                {
                    var table = new TablePrinter("Id", "Name", "Path", "Port", "Url", "State");
                    table.AddRow(result.Result.Id, result.Result.Name, result.Result.Path, result.Result.Port,
                        result.Result.Url, result.Result.State);
                    table.Print();

                    return result.Result;
                }

                if (result is { IsSuccess: false })
                {
                    throw new Exception(result.Error.Message);
                }

                throw new Exception(response.ErrorMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<Site> CreateSite(CreateSiteInput input)
        {
            try
            {
                var zipFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, input.Name + ".zip");
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }

                ZipFile.CreateFromDirectory(input.FilePath, zipFilePath);
                input.FilePath = zipFilePath;

                var response =
                    await HttpManager.CreateFromForm(_host + ":" + _port, "/iis/site", Method.Post, input);
                if (!response.IsSuccessful) throw new Exception(response.ErrorMessage);

                var result = JsonSerializer.Deserialize<ResponseType<Site>>(response.Content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                if (result is { IsSuccess: true })
                {
                    var table = new TablePrinter("Id", "Name", "Path", "Port", "Url", "State");
                    table.AddRow(result.Result.Id, result.Result.Name, result.Result.Path, result.Result.Port,
                        result.Result.Url ?? "", result.Result.State ?? "");
                    table.Print();

                    if (File.Exists(zipFilePath))
                    {
                        File.Delete(zipFilePath);
                    }

                    return result.Result;
                }

                if (result is { IsSuccess: false })
                {
                    if (File.Exists(zipFilePath))
                    {
                        File.Delete(zipFilePath);
                    }

                    throw new Exception(result.Error.Message);
                }

                throw new Exception(response.ErrorMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<Site> DeploySite(DeploySiteInput input)
        {
            try
            {
                await StopPool(input.AppPoolName);

                var zipFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, input.Id + ".zip");
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }

                ZipFile.CreateFromDirectory(input.FilePath, zipFilePath);
                input.FilePath = zipFilePath;

                var success = false;

                var response =
                    await HttpManager.DeployFromForm(_host + ":" + _port, $"/iis/site/{input.Id}/deploy",
                        Method.Post,
                        input);
                if (!response.IsSuccessful) throw new Exception(response.ErrorMessage);

                var result = JsonSerializer.Deserialize<ResponseType<Site>>(response.Content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                if (result is { IsSuccess: true })
                {
                    success = true;

                    var table = new TablePrinter("Id", "Name", "Path", "Port", "Url", "State");
                    table.AddRow(result.Result.Id, result.Result.Name, result.Result.Path, result.Result.Port,
                        result.Result.Url ?? "", result.Result.State ?? "");
                    table.Print();

                    if (File.Exists(zipFilePath))
                    {
                        File.Delete(zipFilePath);
                    }

                    await StartPool(input.AppPoolName);
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<Site> StartSite(StartSiteInput startSiteInput)
        {
            try
            {
                var response =
                    await HttpManager.Request(_host + ":" + _port, $"/iis/site/{startSiteInput.Id}/start", Method.Post,
                        startSiteInput);
                if (!response.IsSuccessful) throw new Exception(response.ErrorMessage);

                var result = JsonSerializer.Deserialize<ResponseType<Site>>(response.Content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                if (result is { IsSuccess: true })
                {
                    var table = new TablePrinter("Id", "Name", "Path", "Port", "Url", "State");
                    table.AddRow(result.Result.Id, result.Result.Name, result.Result.Path, result.Result.Port,
                        result.Result.Url, result.Result.State);
                    table.Print();

                    return result.Result;
                }

                if (result is { IsSuccess: false })
                {
                    throw new Exception(result.Error.Message);
                }

                throw new Exception(response.ErrorMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<Site> StopSite(StopSiteInput stopSiteInput)
        {
            try
            {
                var response =
                    await HttpManager.Request(_host + ":" + _port, $"/iis/site/{stopSiteInput.Id}/stop", Method.Post,
                        stopSiteInput);
                if (!response.IsSuccessful) throw new Exception(response.ErrorMessage);

                var result = JsonSerializer.Deserialize<ResponseType<Site>>(response.Content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                if (result is { IsSuccess: true })
                {
                    var table = new TablePrinter("Id", "Name", "Path", "Port", "Url", "State");
                    table.AddRow(result.Result.Id, result.Result.Name, result.Result.Path, result.Result.Port,
                        result.Result.Url, result.Result.State);
                    table.Print();

                    return result.Result;
                }

                if (result is { IsSuccess: false })
                {
                    throw new Exception(result.Error.Message);
                }

                throw new Exception(response.ErrorMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<Pool> StopPool(string name)
        {
            try
            {
                var response =
                    await HttpManager.Request(_host + ":" + _port, $"/iis/pool/{name}/stop", Method.Post, null);
                if (!response.IsSuccessful) throw new Exception(response.ErrorMessage);

                var result = JsonSerializer.Deserialize<ResponseType<Pool>>(response.Content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                if (result is { IsSuccess: true })
                {
                    Console.WriteLine($"Stopped {name} App Pool.");
                    return result.Result;
                }

                if (result is { IsSuccess: false })
                {
                    throw new Exception(result.Error.Message);
                }

                throw new Exception(response.ErrorMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<Pool> StartPool(string name)
        {
            try
            {
                var response =
                    await HttpManager.Request(_host + ":" + _port, $"/iis/pool/{name}/start", Method.Post, null);
                if (!response.IsSuccessful) throw new Exception(response.ErrorMessage);

                var result = JsonSerializer.Deserialize<ResponseType<Pool>>(response.Content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                if (result is { IsSuccess: true })
                {
                    Console.WriteLine($"Started {name} App Pool.");

                    return result.Result;
                }

                if (result is { IsSuccess: false })
                {
                    throw new Exception(result.Error.Message);
                }

                throw new Exception(response.ErrorMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}