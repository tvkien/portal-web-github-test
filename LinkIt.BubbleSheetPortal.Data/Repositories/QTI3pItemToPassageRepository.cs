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
    public class QTI3pItemToPassageRepository : IRepository<QTI3pItemToPassage>
    {
        private readonly Table<QTI3pItemToPassageEntity> table;

        public QTI3pItemToPassageRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pItemToPassageEntity>();
        }

        public IQueryable<QTI3pItemToPassage> Select()
        {
            return table.Select(x => new QTI3pItemToPassage
                                {
                                    QTI3pItemToPassageId = x.QTI3pItemToPassageID,
                                    Qti3pItemId = x.QTI3pItemID,
                                    Qti3pItemPassageId = x.QTI3pItemPassageID
                                });
        }

        public void Save(QTI3pItemToPassage item)
        {
            var entity = table.FirstOrDefault(x => x.QTI3pItemToPassageID.Equals(item.QTI3pItemToPassageId));

            if (entity.IsNull())
            {
                entity = new QTI3pItemToPassageEntity();
                table.InsertOnSubmit(entity);
            }
            entity.QTI3pItemID = item.Qti3pItemId;
            entity.QTI3pItemPassageID = item.Qti3pItemPassageId;

            table.Context.SubmitChanges();
            item.QTI3pItemToPassageId = entity.QTI3pItemToPassageID;
        }

        public void Delete(QTI3pItemToPassage item)
        {
            var entity = table.FirstOrDefault(x => x.QTI3pItemToPassageID.Equals(item.QTI3pItemToPassageId));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
