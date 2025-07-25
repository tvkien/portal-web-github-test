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
    public class QTI3pFleschKinkaidRepository : IReadOnlyRepository<QTI3pFleschKinkaid>
    {
        private readonly Table<QTI3pFleschKinkaidEntity> table;

        public QTI3pFleschKinkaidRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pFleschKinkaidEntity>();
        }

        public IQueryable<QTI3pFleschKinkaid> Select()
        {
            return table.Select(x => new QTI3pFleschKinkaid
                                {
                                    FleschKincaidID = x.FleschKincaidID,
                                    Name = x.Name,
                                    sOrder = x.sOrder
                                });
        }
    }

}
