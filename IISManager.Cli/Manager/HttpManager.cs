using System.Text.Json;
using IISManager.Cli.Models.Dtos;
using RestSharp;

namespace IISManager.Cli.Manager;

public static class HttpManager
{
    public static async Task<RestResponse> Request(string urlSection, string endpoint, Method method, object body)
    {
        var options = new RestClientOptions(urlSection + endpoint)
        {
            Timeout = TimeSpan.FromMilliseconds(-1)
        };

        var client = new RestClient(options);
        var request = new RestRequest(string.Empty, method);
        request.AddHeader("Content-Type", "application/json");
        if (body != null)
        {
            request.AddParameter("application/json", JsonSerializer.Serialize(body), ParameterType.RequestBody);
        }

        return await client.ExecuteAsync(request);
    }

    public static async Task<RestResponse> CreateFromForm(string urlSection, string endpoint, Method method,
        CreateSiteInput body)
    {
        var options = new RestClientOptions(urlSection + endpoint)
        {
            Timeout = TimeSpan.FromMilliseconds(-1)
        };

        var client = new RestClient(options);
        var request = new RestRequest(string.Empty, method);
        if (body != null)
        {
            // File upload
            if (File.Exists(body.FilePath))
            {
                request.AddFile("file", body.FilePath);
            }

            // Form fields
            request.AddParameter("name", body.Name);
            request.AddParameter("port", body.Port);
            request.AddParameter("bindingInformation", body.BindingInformation);
        }

        return await client.ExecuteAsync(request);
    }

    public static async Task<RestResponse> DeployFromForm(string urlSection, string endpoint, Method method,
        DeploySiteInput body)
    {
        var options = new RestClientOptions(urlSection + endpoint)
        {
            Timeout = System.Threading.Timeout.InfiniteTimeSpan
        };

        var client = new RestClient(options);
        var request = new RestRequest(string.Empty, method);

        if (body != null)
        {
            if (File.Exists(body.FilePath))
            {
                request.AddFile("file", body.FilePath);
            }

            request.AddParameter("id", body.Id);
        }

        return await client.ExecuteAsync(request);
    }
}