using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTI3pItemStateStandardRepository : IRepository<QTI3pItemStateStandard>
    {
        private readonly Table<QTI3pItemStateStandardEntity> table;

        public QTI3pItemStateStandardRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pItemStateStandardEntity>();
        }

        public IQueryable<QTI3pItemStateStandard> Select()
        {
            return table.Select(x => new QTI3pItemStateStandard
                                    {
                                        Qti3pItemId = x.QTI3pItemID,
                                        Qti3pItemStateStandardId = x.QTI3pItemStateStandardID,
                                        StateStandardId = x.StateStandardID
                                    });
        }

        public void Save(QTI3pItemStateStandard item)
        {
            var entity = table.FirstOrDefault(x => x.QTI3pItemStateStandardID.Equals(item.Qti3pItemStateStandardId));

            if (entity.IsNull())
            {
                entity = new QTI3pItemStateStandardEntity();
                table.InsertOnSubmit(entity);
            }
            entity.QTI3pItemID = item.Qti3pItemId;
            entity.StateStandardID = item.StateStandardId;            

            table.Context.SubmitChanges();
            item.Qti3pItemStateStandardId = entity.QTI3pItemStateStandardID;
        }

        public void Delete(QTI3pItemStateStandard item)
        {
            var entity = table.FirstOrDefault(x => x.QTI3pItemStateStandardID.Equals(item.Qti3pItemStateStandardId));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
