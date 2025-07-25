using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetProcessingRequestSheetRepository : IReadOnlyRepository<BubbleSheetProcessingRequestSheet>
    {
        private readonly Table<BubbleSheetProcessingRequestSheetEntity> _tableBBSRequestSheet;

        public BubbleSheetProcessingRequestSheetRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _tableBBSRequestSheet = BubbleSheetDataContext.Get(connectionString).GetTable<BubbleSheetProcessingRequestSheetEntity>();
        }

        public IQueryable<BubbleSheetProcessingRequestSheet> Select()
        {
            return _tableBBSRequestSheet.Select(x => new BubbleSheetProcessingRequestSheet
            {
                RequestSheetId = x.RequestSheetId,
                SheetJobTicket = x.SheetJobTicket,
                NumberOfGraphExtraPages = x.NumberOfGraphExtraPages ?? 0,
                NumberOfLinedExtraPages = x.NumberOfLinedExtraPages ?? 0,
                NumberOfPlainExtraPages = x.NumberOfPlainExtraPages ?? 0
            });
        }
    }
}
