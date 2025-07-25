using System.Linq;
using Envoc.Core.Shared.Data;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.Interfaces
{
    public interface IBubbleSheetFileRepository : IReadOnlyRepository<BubbleSheetFile>
    {
        IQueryable<BubbleSheetFile> SelectFromBubbleSheetFileTicketView();
        void Save(BubbleSheetFile item);
        int SATGetDistinctPageCount(string ticket, int classID, int studentID);

        GenericBubbleSheet GetGenericFileInfor(int bubblesheetFileId);
        IQueryable<BubbleSheetFileSub> GetBubbleSheetLatestFille(string ticket, int classID);
        void Save(List<BubbleSheetFile> items);
         void Delete(BubbleSheetFile item);
        void SaveBubbleSheetFileCorrections(List<int> ids);
        BubbleSheetProcessingReadResult GetBubbleSheetProcessingReadResult(string inputPath, string urlSafeOutputFile);
    }
}
