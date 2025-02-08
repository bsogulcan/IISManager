using System.IO.Compression;
using IISManager.Managers.FileManager;
using IISManager.Models.Dtos;
using Microsoft.Web.Administration;
using Site = IISManager.Models.Site;


namespace IISManager.Managers.SiteManagers;

public class SiteManager : ISiteManager
{
    private readonly IFileManager _fileManager;
    private const string _publishesFilePath = @"C:\inetpub\wwwroot\IISManager_Publishes";
    private const string _backUpFilePath = @"C:\inetpub\wwwroot\IISManager_Backups";

    public SiteManager(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public Site Create(CreateSiteInput input)
    {
        try
        {
            using (var serverMgr = new ServerManager())
            {
                var sites = serverMgr.Sites;

                if (sites.Any(x => x.Name.ToLower() == input.Name.ToLower()))
                    throw new Exception("Site existing with Name:" + input.Name);

                var appPools = serverMgr.ApplicationPools;
                if (!appPools.Any(x => x.Name.ToLower() == input.Name.ToLower()))
                {
                    serverMgr.ApplicationPools.Add(input.Name);
                }

                string sitePath = Path.Combine("C:", "inetpub", "wwwroot", input.Name);
                var newSite = new Site()
                {
                    Name = input.Name,
                    Port = input.Port,
                    Path = sitePath,
                };

                var publishFile = _fileManager.UploadFileAndGetFullPath(_publishesFilePath, input.File);

                if (!_fileManager.CheckFolderExist(newSite.Path))
                {
                    _fileManager.CreateFolder(newSite.Path);
                }

                ZipFile.ExtractToDirectory(publishFile, newSite.Path);

                var iisSite = serverMgr.Sites.Add(newSite.Name, newSite.Path, newSite.Port);
                iisSite.ApplicationDefaults.ApplicationPoolName = iisSite.Name;
                iisSite.Bindings.First().BindingInformation = string.IsNullOrEmpty(input.BindingInformation)
                    ? "*:" + input.Port + ":"
                    : input.BindingInformation + ":" + input.Port + ":";

                serverMgr.CommitChanges();
                try
                {
                    newSite.Id = iisSite.Id;
                    newSite.Url = iisSite.Bindings.First().BindingInformation
                        .Substring(0, iisSite.Bindings.First().BindingInformation.Length - 1);
                    newSite.State = SiteObjectStateConverter.GetString(iisSite.State);
                }
                catch (Exception e)
                {
                    // ignored
                }

                return newSite;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Site Update(UpdateSiteInput input)
    {
        try
        {
            using (var serverMgr = new ServerManager())
            {
                var site = serverMgr.Sites.FirstOrDefault(x => x.Id == input.Id);
                if (site == null)
                {
                    throw new Exception("Site not found! Id:" + input.Id);
                }

                site.Name = input.Name;
                site.Bindings.First().BindingInformation = input.bindingInformation;
                serverMgr.CommitChanges();

                return new Site(site);
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public Site Deploy(DeploySiteInput input)
    {
        try
        {
            using (var serverMgr = new ServerManager())
            {
                var site = serverMgr.Sites.FirstOrDefault(x => x.Id == input.Id);
                if (site == null)
                {
                    throw new Exception("Site not found! Id:" + input.Id);
                }

                if (site.State == ObjectState.Started)
                {
                    throw new Exception(
                        "Current State is Started. Cannot deploy started sites, please stop it first!");
                }

                var sitePath = site.Applications.First().VirtualDirectories.First().PhysicalPath;
                var publishFile = _fileManager.UploadFileAndGetFullPath(_publishesFilePath, input.File);
                var backUpFile = Path.Combine(_backUpFilePath,
                    DateTime.Now.ToString("yyyymmddMMss") + '-' + site.Name);

                if (!_fileManager.CheckFolderExist(_backUpFilePath))
                {
                    _fileManager.CreateFolder(_backUpFilePath);
                }

                ZipFile.CreateFromDirectory(sitePath, backUpFile);
                SetAttributesNormal(new DirectoryInfo(sitePath));

                Directory.Delete(sitePath, true);

                ZipFile.ExtractToDirectory(publishFile, sitePath, overwriteFiles: true);
                serverMgr.CommitChanges();

                return new Site(site);
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public Site Stop(long id)
    {
        try
        {
            using (var serverMgr = new ServerManager())
            {
                var site = serverMgr.Sites.FirstOrDefault(x => x.Id == id);
                if (site == null)
                {
                    throw new Exception("Site not found! Id:" + id);
                }

                site.Stop();
                serverMgr.CommitChanges();

                return new Site(site);
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public Site Start(long id)
    {
        try
        {
            using (var serverMgr = new ServerManager())
            {
                var site = serverMgr.Sites.FirstOrDefault(x => x.Id == id);
                if (site == null)
                {
                    throw new Exception("Site not found! Id:" + id);
                }

                site.Start();
                serverMgr.CommitChanges();

                return new Site(site);
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    private void SetAttributesNormal(DirectoryInfo dir)
    {
        foreach (var subDir in dir.GetDirectories())
        {
            SetAttributesNormal(subDir);
            subDir.Attributes = FileAttributes.Normal;
        }

        foreach (var file in dir.GetFiles())
        {
            file.Attributes = FileAttributes.Normal;
        }
    }
}