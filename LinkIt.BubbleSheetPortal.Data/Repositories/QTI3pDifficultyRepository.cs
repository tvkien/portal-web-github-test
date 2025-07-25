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
    public class QTI3pDifficultyRepository : IReadOnlyRepository<QTI3pDifficulty>
    {
        private readonly Table<QTI3pDifficultyEntity> table;

        public QTI3pDifficultyRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pDifficultyEntity>();
        }

        public IQueryable<QTI3pDifficulty> Select()
        {
            return table.Select(x => new QTI3pDifficulty
                                {
                                    ItemDifficultyID = x.ItemDifficultyID,
                                    Name =  x.Name
                                });
        }
    }
}
