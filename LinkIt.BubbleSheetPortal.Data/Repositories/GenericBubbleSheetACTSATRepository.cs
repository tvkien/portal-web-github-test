using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class GenericBubbleSheetACTSATRepository : IReadOnlyRepository<GenericBubbleSheetACTSAT>
    {
        private readonly Table<GenericBubbleSheetACTSATView> table;

        public GenericBubbleSheetACTSATRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = BubbleSheetDataContext.Get(connectionString).GetTable<GenericBubbleSheetACTSATView>();
        }

        public IQueryable<GenericBubbleSheetACTSAT> Select()
        {
            return table.Select(x => new GenericBubbleSheetACTSAT
                {
                    BubbleSheetId = x.BubblesheetID,
                    BubbleSheetFileId = x.BubbleSheetFileID.GetValueOrDefault(),
                    ClassID = x.ClassID,
                    OutputFileName = x.OutputFileName,
                    StudentID = x.StudentID ?? 0,
                    Ticket = x.Ticket,
                    InputFileName = x.InputFileName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    VirtualTestSubTypeId = x.VirtualTestSubTypeID ?? 0,
                    ClassIds = x.ClassIDs
                });
        }
    }
}
