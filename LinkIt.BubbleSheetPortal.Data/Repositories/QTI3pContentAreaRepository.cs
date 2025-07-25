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
    public class QTI3pContentAreaRepository : IReadOnlyRepository<QTI3pContentArea>
    {
        private readonly Table<QTI3pContentAreaEntity> table;

        public QTI3pContentAreaRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pContentAreaEntity>();
        }

        public IQueryable<QTI3pContentArea> Select()
        {
            return table.Select(x => new QTI3pContentArea
            {
                                    ContentAreaId = x.ContentAreaID,
                                    Name =  x.Name
                                });
        }
    }
}
