using Microsoft.AspNetCore.Http;

namespace IISManager.Managers.FileManager
{
    public interface IFileManager
    {
        bool CheckFolderExist(string path);
        void CreateFolder(string path);
        string UploadFileAndGetFullPath(string path, IFormFile formFile);
    }
}