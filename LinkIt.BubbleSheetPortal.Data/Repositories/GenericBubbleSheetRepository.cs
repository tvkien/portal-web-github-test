using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class GenericBubbleSheetRepository : IReadOnlyRepository<GenericBubbleSheet>
    {
        private readonly Table<GenericBubbleSheetView> table;

        public GenericBubbleSheetRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = BubbleSheetDataContext.Get(connectionString).GetTable<GenericBubbleSheetView>();
        }

        public IQueryable<GenericBubbleSheet> Select()
        {
            return table.Select(x => new GenericBubbleSheet
                {
                    BubbleSheetId = x.BubblesheetID,
                    BubbleSheetFileId = x.BubbleSheetFileID,
                    ClassID = x.ClassID.GetValueOrDefault(),
                    OutputFileName = x.OutputFileName,
                    StudentID = x.StudentID ?? 0,
                    Ticket = x.Ticket,
                    InputFileName = x.InputFileName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PageNumber = x.PageNumber.GetValueOrDefault()
                });
        }
    }
}
