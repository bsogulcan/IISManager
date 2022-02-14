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
    }
}