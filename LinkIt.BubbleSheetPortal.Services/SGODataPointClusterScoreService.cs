using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SGODataPointClusterScoreService
    {
        private readonly IRepository<SGODataPointClusterScore> sgoDataPointClusterScoreRepository;

        public SGODataPointClusterScoreService(IRepository<SGODataPointClusterScore> sgoDataPointClusterScoreRepository)
        {
            this.sgoDataPointClusterScoreRepository = sgoDataPointClusterScoreRepository;
        }

        public IQueryable<SGODataPointClusterScore> GetDataPointClusterScoreBySGODataPointID(int sGODataPointID)
        {
            return sgoDataPointClusterScoreRepository.Select().Where(x => x.SGODataPointID == sGODataPointID);
        }

        public void Save(SGODataPointClusterScore sGoDataPointClusterScore)
        {
            sgoDataPointClusterScoreRepository.Save(sGoDataPointClusterScore);
        }

        public void Delete(SGODataPointClusterScore sGoDataPointClusterScore)
        {
            sgoDataPointClusterScoreRepository.Delete(sGoDataPointClusterScore);
        }
    }
}