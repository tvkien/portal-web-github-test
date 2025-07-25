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
    public class QTI3pWordCountRepository : IReadOnlyRepository<QTI3pWordCount>
    {
        private readonly Table<QTI3pWordCountEntity> table;

        public QTI3pWordCountRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pWordCountEntity>();
        }

        public IQueryable<QTI3pWordCount> Select()
        {
            return table.Select(x => new QTI3pWordCount
                                {
                                    Name = x.Name,
                                    sOrder = x.sOrder,
                                    WordCountID = x.WordCountID
                                });
        }
    }
}
