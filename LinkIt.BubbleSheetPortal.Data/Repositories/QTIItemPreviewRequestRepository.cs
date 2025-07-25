using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIItemPreviewRequestRepository : IRepository<QTIItemPreviewRequest>
    {
        private readonly Table<QTIItemPreviewRequestEntity> table;

        public QTIItemPreviewRequestRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<QTIItemPreviewRequestEntity>();
            Mapper.CreateMap<QTIItemPreviewRequest, QTIItemPreviewRequestEntity>();
        }

        public IQueryable<QTIItemPreviewRequest> Select()
        {
            return table.Select(x => new QTIItemPreviewRequest
            {
                CreatedDate = x.CreatedDate,
                QTIItemPreviewRequestId = x.QTIItemPreviewRequestID,
                XmlContent = x.XmlContent,
                VirtualTestId = x.VirtualTestID
            });
        }

        public void Save(QTIItemPreviewRequest item)
        {
            if (string.IsNullOrEmpty(item.QTIItemPreviewRequestId))
            {
                item.QTIItemPreviewRequestId = Guid.NewGuid().ToString();
            }

            var entity = table.FirstOrDefault(x => x.QTIItemPreviewRequestID.Equals(item.QTIItemPreviewRequestId));

            if (entity.IsNull())
            {
                entity = new QTIItemPreviewRequestEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
        }

        private void MapModelToEntity(QTIItemPreviewRequest model, QTIItemPreviewRequestEntity entity)
        {
            entity.CreatedDate = model.CreatedDate;
            entity.QTIItemPreviewRequestID = model.QTIItemPreviewRequestId;
            entity.XmlContent = model.XmlContent;
            entity.VirtualTestID = model.VirtualTestId;
        }

        public void Delete(QTIItemPreviewRequest item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemPreviewRequestID.Equals(item.QTIItemPreviewRequestId));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}