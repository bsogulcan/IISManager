using System.Net.Http.Headers;

namespace IISManager.Managers.FileManager;

public class FileManager : IFileManager
{
    public bool CheckFolderExist(string path)
    {
        return Directory.Exists(path);
    }

    public void CreateFolder(string path)
    {
        Directory.CreateDirectory(path);
    }

    public string UploadFileAndGetFullPath(string path, IFormFile formFile)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var fileName = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName?.Trim('"');
        fileName = DateTime.Now.ToString("yyyymmddMMss") + '-' + fileName;
        var fullPath = Path.Combine(path, fileName);

        using var fileStream = new FileStream(fullPath, FileMode.Create);
        formFile.CopyTo(fileStream);

        return fullPath;
    }
}