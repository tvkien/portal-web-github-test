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
    public class QTI3pItemDOKRepository : IRepository<QTI3pItemDOK>
    {
        private readonly Table<QTI3pItemDOKEntity> table;

        public QTI3pItemDOKRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pItemDOKEntity>();
        }

        public IQueryable<QTI3pItemDOK> Select()
        {
            return table.Select(x => new QTI3pItemDOK
            {
                                        Qti3pItemId = x.QTI3pItemID,
                                        Qti3pDOK = x.QTI3pDOK,
                                        Qti3pItemDOKId = x.QTI3pItemDOKID
                                    });
        }

        public void Save(QTI3pItemDOK item)
        {
            var entity = table.FirstOrDefault(x => x.QTI3pItemDOKID.Equals(item.Qti3pItemDOKId));

            if (entity.IsNull())
            {
                entity = new QTI3pItemDOKEntity();
                table.InsertOnSubmit(entity);
            }
            entity.QTI3pItemID = item.Qti3pItemId;
            entity.QTI3pDOK = item.Qti3pDOK;            

            table.Context.SubmitChanges();
            item.Qti3pItemDOKId = entity.QTI3pItemDOKID;
        }

        public void Delete(QTI3pItemDOK item)
        {
            throw new NotImplementedException();
        }
    }
}
