using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TLDS
{
    public interface ITLDSDownloadQueueRepository : IRepository<TLDSDownloadQueue>
    {
        TLDSDownloadQueue GetByFileName(string fileName);
    }
}
