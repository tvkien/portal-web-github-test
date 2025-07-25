using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIItemHistoryRepository : IQTIItemHistoryRepository
    {
        private readonly Table<QTIItemHistoryEntity> table;
        private readonly AssessmentDataContext _assessmentDataContext;

        public QTIItemHistoryRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<QTIItemHistoryEntity>();

            _assessmentDataContext = AssessmentDataContext.Get(connectionString);
        }

        public IQueryable<QTIItemHistory> Select()
        {
            return table.Select(x => new QTIItemHistory
            {
                QTIItemHistoryID = x.QTIItemHistoryID,
                ChangedDate = x.ChangedDate,
                QTIItemID = x.QTIItemID,
                XmlContent = x.XmlContent,
                AuthorID = x.AuthorID
            });
        }

        public void Save(QTIItemHistory item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemHistoryID.Equals(item.QTIItemHistoryID));

            if (entity.IsNull())
            {
                entity = new QTIItemHistoryEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            table.Context.SubmitChanges();
            item.QTIItemID = entity.QTIItemID;
        }

        private void MapModelToEntity(QTIItemHistoryEntity entity, QTIItemHistory item)
        {
            entity.QTIItemHistoryID = item.QTIItemHistoryID;
            entity.ChangedDate = item.ChangedDate;
            entity.QTIItemID = item.QTIItemID;
            entity.XmlContent = item.XmlContent;
            entity.AuthorID = item.AuthorID;
        }
    }
}
