using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTI3pTextTypeRepository : IRepository<QTI3pTextType>
    {
         private readonly Table<QTI3pTextTypeEntity> table;

         public QTI3pTextTypeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pTextTypeEntity>();
        }

        public IQueryable<QTI3pTextType> Select()
        {
            return table.Select(x => new QTI3pTextType
                                {
                                    Name = x.Name,
                                    TypeID = x.TypeID ?? 0,
                                    TextTypeID = x.TextTypeID
                                });
        }

        public void Save(QTI3pTextType item)
        {
            var entity = table.FirstOrDefault(x => x.TextTypeID.Equals(item.TextTypeID));

            if (entity.IsNull())
            {
                entity = new QTI3pTextTypeEntity();
                table.InsertOnSubmit(entity);
            }
            entity.Name = item.Name;
            entity.TypeID = item.TypeID;

            table.Context.SubmitChanges();
            item.TextTypeID = entity.TextTypeID;
        }

        public void Delete(QTI3pTextType item)
        {
            throw new NotImplementedException();
        }
    }
}
