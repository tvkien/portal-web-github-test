using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SGOStudentDataPointService
    {
        private readonly IRepository<SGOStudentDataPoint> sgoStudentDataPointRepository;

        public SGOStudentDataPointService(IRepository<SGOStudentDataPoint> sgoStudentDataPointRepository)
        {
            this.sgoStudentDataPointRepository = sgoStudentDataPointRepository;
        }

        public IQueryable<SGOStudentDataPoint> GetStudentDataPointBySGODataPointID(int sGODataPointID)
        {
            return sgoStudentDataPointRepository.Select().Where(x => x.SGODataPointID == sGODataPointID);
        }

        public void Save(SGOStudentDataPoint sGOStudentDataPoint)
        {
            sgoStudentDataPointRepository.Save(sGOStudentDataPoint);
        }

        public void Delete(SGOStudentDataPoint sGOStudentDataPoint)
        {
            sgoStudentDataPointRepository.Delete(sGOStudentDataPoint);
        }
    }
}