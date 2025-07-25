using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetErrorViewRepository : IReadOnlyRepository<BubbleSheetError>
    {
        private readonly Table<BubbleSheetErrorView> table;

        public BubbleSheetErrorViewRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = BubbleSheetDataContext.Get(connectionString).GetTable<BubbleSheetErrorView>();
        }

        public IQueryable<BubbleSheetError> Select()
        {
            return table.Select(x => new BubbleSheetError
                                         {
                                             BubbleSheetErrorId = x.ID,
                                             BubbleSheetId = x.BubbleSheetID,
                                             FileName = x.FileName,
                                             CreatedDate = x.CreatedDate,
                                             Message = x.Message,
                                             UploadedBy = x.UploadedBy,
                                             RelatedImage = x.RelatedImage,
                                             UserId = x.UserID,
                                             DistrictId = x.DistrictID.GetValueOrDefault(),
                                             ErrorCode = x.ErrorCode == null ? -1 : Convert.ToInt32(x.ErrorCode),
                                             VirtualTestId = x.VirtualTestID
                                         });
        }
    }
}