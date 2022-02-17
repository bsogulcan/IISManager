using System;
using System.IO;
using System.Linq;
using IISManager.Managers.FileManager;
using IISManager.Models.Dtos;
using Microsoft.Web.Administration;
using Site = IISManager.Models.Site;
using System.IO.Compression;

namespace IISManager.Managers.SiteManagers
{
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
                    Path = sitePath
                };

                var publishFile = _fileManager.UploadFileAndGetFullPath(_publishesFilePath, input.File);

                if (!_fileManager.CheckFolderExist(newSite.Path))
                {
                    _fileManager.CreateFolder(newSite.Path);
                }

                ZipFile.ExtractToDirectory(publishFile, newSite.Path);

                var iisSite = serverMgr.Sites.Add(newSite.Name, newSite.Path, newSite.Port);
                iisSite.ApplicationDefaults.ApplicationPoolName = iisSite.Name;

                serverMgr.CommitChanges();
                return newSite;
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

                    var sitePath = site.Applications.First().VirtualDirectories.First().PhysicalPath;
                    var publishFile = _fileManager.UploadFileAndGetFullPath(_publishesFilePath, input.File);
                    var backUpFile = Path.Combine(_backUpFilePath,
                        DateTime.Now.ToString("yyyymmddMMss") + '-' + site.Name);

                    if (!_fileManager.CheckFolderExist(_backUpFilePath))
                    {
                        _fileManager.CreateFolder(_backUpFilePath);
                    }

                    ZipFile.CreateFromDirectory(sitePath, backUpFile);

                    site.Stop();
                    serverMgr.CommitChanges();
                    ZipFile.ExtractToDirectory(publishFile, sitePath, overwriteFiles: true);
                    site.Start();

                    return new Site(site);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}