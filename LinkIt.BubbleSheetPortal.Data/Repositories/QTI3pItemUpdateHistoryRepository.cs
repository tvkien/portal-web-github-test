using System;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTI3pItemUpdateHistoryRepository : IRepository<QTI3pItemUpdateHistory>
    {
        private readonly Table<QTI3pItemUpdateHistoryEntity> table;

        public QTI3pItemUpdateHistoryRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pItemUpdateHistoryEntity>();
            Mapper.CreateMap<QTI3pItemUpdateHistory, QTI3pItemUpdateHistoryEntity>();
        }

        public IQueryable<QTI3pItemUpdateHistory> Select()
        {
            return table.Select(x => new QTI3pItemUpdateHistory
            {
                QTI3pItemUpdateHistoryID = x.QTI3pItemUpdateHistoryID,
                QTI3pItemID = x.QTI3pItemID??0,
                ColumnName = x.ColumnName,
                OldValue = x.OldValue,
                NewValue = x.NewValue
            });
        }

        public void Save(QTI3pItemUpdateHistory item)
        {
            var entity = table.FirstOrDefault(x => x.QTI3pItemUpdateHistoryID.Equals(item.QTI3pItemUpdateHistoryID));

            if (entity.IsNull())
            {
                entity = new QTI3pItemUpdateHistoryEntity();
                table.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.QTI3pItemUpdateHistoryID = entity.QTI3pItemUpdateHistoryID;
        }

        public void Delete(QTI3pItemUpdateHistory item)
        {
            throw new NotImplementedException();
        }
    }
}
