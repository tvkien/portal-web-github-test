using System;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTI3pItemDependencyDeleteHistoryRepository : IRepository<QTI3pItemDependencyDeleteHistory>
    {
        private readonly Table<QTI3pItemDependencyDeleteHistoryEntity> table;

        public QTI3pItemDependencyDeleteHistoryRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pItemDependencyDeleteHistoryEntity>();
            Mapper.CreateMap<QTI3pItemDependencyDeleteHistory, QTI3pItemDependencyDeleteHistoryEntity>();
        }

        public IQueryable<QTI3pItemDependencyDeleteHistory> Select()
        {
            return table.Select(x => new QTI3pItemDependencyDeleteHistory
            {
                QTI3pItemDependencyDeleteHistoryID = x.QTI3pItemDependencyDeleteHistoryID,
                QTI3pItemID = x.QTI3pItemID??0,
                TableName = x.TableName,
                DenpendencyEntityValue = x.DenpendencyEntityValue
            });
        }

        public void Save(QTI3pItemDependencyDeleteHistory item)
        {
            var entity = table.FirstOrDefault(x => x.QTI3pItemDependencyDeleteHistoryID.Equals(item.QTI3pItemDependencyDeleteHistoryID));

            if (entity.IsNull())
            {
                entity = new QTI3pItemDependencyDeleteHistoryEntity();
                table.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.QTI3pItemDependencyDeleteHistoryID = entity.QTI3pItemDependencyDeleteHistoryID;
        }

        public void Delete(QTI3pItemDependencyDeleteHistory item)
        {
            throw new NotImplementedException();
        }
    }
}
