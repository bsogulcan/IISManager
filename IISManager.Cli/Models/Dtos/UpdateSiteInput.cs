namespace IISManager.Cli.Models.Dtos;

public class UpdateSiteInput
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string bindingInformation { get; set; }
}