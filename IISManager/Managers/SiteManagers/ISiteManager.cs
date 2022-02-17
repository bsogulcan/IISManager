using IISManager.Models;
using IISManager.Models.Dtos;

namespace IISManager.Managers.SiteManagers
{
    public interface ISiteManager
    {
        public Site Create(CreateSiteInput input);
        public Site Update(UpdateSiteInput input);
        public Site Deploy(DeploySiteInput input);
    }
}