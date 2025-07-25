using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ISGODataPointRepository : IRepository<SGODataPoint>
    {
        SGODataPoint GetByID(int sgoDataPointId);
    }
}
