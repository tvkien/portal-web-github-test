using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SGODataPointFilterService
    {
        private readonly IRepository<SGODataPointFilter> sgoDataPointFilterRepository;

        public SGODataPointFilterService(IRepository<SGODataPointFilter> sgoDataPointFilterRepository)
        {
            this.sgoDataPointFilterRepository = sgoDataPointFilterRepository;
        }

        public IQueryable<SGODataPointFilter> GetDataPointFilterBySGODataPointID(int sGODataPointID)
        {
            return sgoDataPointFilterRepository.Select().Where(x => x.SGODataPointID == sGODataPointID);
        }

        public void Save(SGODataPointFilter sGoDataPointFilter)
        {
            sgoDataPointFilterRepository.Save(sGoDataPointFilter);
        }

        public void Delete(SGODataPointFilter sGoDataPointFilter)
        {
            sgoDataPointFilterRepository.Delete(sGoDataPointFilter);
        }
    }
}