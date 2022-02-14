using IISManager.Models;
using IISManager.Models.Dtos;

namespace IISManager.Managers.SiteManagers
{
    public interface ISiteManager
    {
        public Site Create(CreateSiteInput input);
    }
}