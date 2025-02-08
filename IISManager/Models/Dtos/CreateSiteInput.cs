namespace IISManager.Models.Dtos;

public class CreateSiteInput
{
    public string Name { get; set; }
    public int Port { get; set; }
    public string Path { get; set; }
    public IFormFile File { get; set; }
    public string BindingInformation { get; set; }
}