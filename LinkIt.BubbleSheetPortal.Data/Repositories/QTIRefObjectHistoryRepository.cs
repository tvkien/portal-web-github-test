using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.QTIRefObject;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIRefObjectHistoryRepository : IRepository<QtiRefObjectHistory>, IQTIRefObjectHistoryRepository
    {
        private readonly Table<QTIRefObjectHistoryEntity> table;
        private readonly AssessmentDataContext _assessmentDataContext;

        public QTIRefObjectHistoryRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<QTIRefObjectHistoryEntity>();

            _assessmentDataContext = AssessmentDataContext.Get(connectionString);
        }

        public IQueryable<QtiRefObjectHistory> Select()
        {
            return table.Select(x => new QtiRefObjectHistory
            {
                QTIRefObjectHistoryId = x.QTIRefObjectHistoryID,
                QTIRefObjectId = x.QTIRefObjectID,
                ChangedDate = x.ChangedDate,
                XmlContent = x.XmlContent,
                AuthorId = x.AuthorID,
            });
        }

        public void Save(QtiRefObjectHistory item)
        {
            var entity = table.FirstOrDefault(x => x.QTIRefObjectHistoryID == item.QTIRefObjectHistoryId);

            if (entity == null)
            {
                entity = new QTIRefObjectHistoryEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            table.Context.SubmitChanges();

            item.QTIRefObjectHistoryId = entity.QTIRefObjectHistoryID;
        }

        public void Delete(QtiRefObjectHistory item)
        {
            var entity = table.FirstOrDefault(x => x.QTIRefObjectHistoryID.Equals(item.QTIRefObjectHistoryId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(QTIRefObjectHistoryEntity entity, QtiRefObjectHistory item)
        {
            entity.QTIRefObjectID = item.QTIRefObjectId;
            entity.ChangedDate = item.ChangedDate;
            entity.XmlContent = item.XmlContent;
            entity.AuthorID = item.AuthorId;
        }
    }
}
