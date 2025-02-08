using IISManager.Cli.Models;
using IISManager.Cli.Models.Dtos;
using IISManagerCli.Enums;
using Microsoft.Extensions.Configuration;
using Sharprompt;

namespace IISManager.Cli;

internal class Program(IConfiguration configuration, Manager.IISManager iisManager)
{
    private List<Site> _sites { get; set; }

    public static async Task Main()
    {
        var configuration = InitializeConfiguration();
        var profile = SelectProfile(configuration);
        var iisManager = CreateIISManager(configuration, profile);

        var program = new Program(configuration, iisManager);
        await program.Run();
    }

    private static IConfiguration InitializeConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    private static string SelectProfile(IConfiguration configuration)
    {
        var profiles = configuration.GetChildren()
            .Select(children => children.Key)
            .ToList();

        return Prompt.Select("Select Server", profiles.ToArray(), pageSize: 3);
    }

    private static IISManager.Cli.Manager.IISManager CreateIISManager(IConfiguration configuration, string profile)
    {
        var hostAddress = configuration.GetSection($"{profile}:IpAddress").Value;
        var port = Convert.ToInt32(configuration.GetSection($"{profile}:Port").Value);
        return new IISManager.Cli.Manager.IISManager(hostAddress, port);
    }

    private async Task Run()
    {
        _sites = await iisManager.GetList();
        
        while (true)
        {
            var processType = Prompt.Select<ProcessType>("Select Process");
            await ProcessCommand(processType);
        }
    }

    private async Task ProcessCommand(ProcessType processType)
    {
        switch (processType)
        {
            case ProcessType.Get:
                await HandleGetSite();
                break;
            case ProcessType.GetAll:
                await HandleGetAllSites();
                break;
            case ProcessType.Create:
                await HandleCreateSite();
                break;
            case ProcessType.Deploy:
                await HandleDeploySite();
                break;
            case ProcessType.Start:
                await HandleStartSite();
                break;
            case ProcessType.Stop:
                await HandleStopSite();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(processType));
        }
    }

    private async Task HandleGetSite()
    {
        var siteId = Prompt.Input<int>("Site Id");
        await iisManager.Get(siteId);
    }

    private async Task HandleGetAllSites()
    {
        Console.Clear();
        _sites = await iisManager.GetList();
    }

    private async Task HandleCreateSite()
    {
        Console.Clear();
        var input = new CreateSiteInput
        {
            Name = Prompt.Input<string>("Site Name"),
            BindingInformation = Prompt.Input<string>("IP Address", "*"),
            Port = Prompt.Input<int>("Port Name"),
            FilePath = Prompt.Input<string>("Publish Folder Path")
        };
        await iisManager.CreateSite(input);
    }

    private async Task HandleDeploySite()
    {
        Console.Clear();
        var input = new DeploySiteInput
        {
            Id = Prompt.Input<long>("Site Id"),
            FilePath = Prompt.Input<string>("Publish Folder Path"),
        };

        input.AppPoolName = _sites.First(x => x.Id == input.Id).AppPoolName;
        await iisManager.DeploySite(input);
    }

    private async Task HandleStartSite()
    {
        var siteId = Prompt.Input<int>("Site Id");
        var input = new StartSiteInput { Id = siteId };
        Console.Clear();
        await iisManager.StartSite(input);
    }

    private async Task HandleStopSite()
    {
        var siteId = Prompt.Input<int>("Site Id");
        var input = new StopSiteInput { Id = siteId };
        Console.Clear();
        await iisManager.StopSite(input);
    }
}