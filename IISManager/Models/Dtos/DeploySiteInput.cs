namespace IISManager.Models.Dtos;

public class DeploySiteInput
{
    public long Id { get; set; }

    public IFormFile File { get; set; }
}