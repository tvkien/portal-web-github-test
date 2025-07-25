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
    public class QTI3pSourceRepository : IReadOnlyRepository<QTI3pSource>
    {
        private readonly Table<QTI3pSourceEntity> table;

        public QTI3pSourceRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pSourceEntity>();
        }

        public IQueryable<QTI3pSource> Select()
        {
            return table.Select(x => new QTI3pSource
                                    {
                                        QTI3pSourceId = x.QTI3pSourceID,
                                        Name = x.Name,
                                        Description = x.Description
                                    });
        }
    }
}
