using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.MultiPartExpression;
using System;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class MultiPartExpressionPreviewRepository : IRepository<MultiPartExpressionPreview>
    {
        private readonly Table<MultiPartExpressionPreviewEntity> _table;
        public MultiPartExpressionPreviewRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<MultiPartExpressionPreviewEntity>();
        }

        public void Delete(MultiPartExpressionPreview item)
        {
            throw new NotImplementedException();
        }

        public void Save(MultiPartExpressionPreview item)
        {
            var entity = _table.FirstOrDefault(x => x.QTIItemPreviewRequestID.Equals(item.MultiPartExpressionPreviewID));

            if (entity.IsNull())
            {
                entity = new MultiPartExpressionPreviewEntity();
                _table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            _table.Context.SubmitChanges();
        }

        private void MapModelToEntity(MultiPartExpressionPreview model, MultiPartExpressionPreviewEntity entity)
        {
            entity.QTIItemPreviewRequestID = model.QTIItemPreviewRequestID;
            entity.Expression = model.Expression;
            entity.EnableElements = model.EnableElements;
            entity.Order = model.Order;
            entity.Rules = model.Rules;
            entity.CreatedDate = DateTime.UtcNow;
        }

        public IQueryable<MultiPartExpressionPreview> Select()
        {
            return _table.Select(o => new MultiPartExpressionPreview
            {
                MultiPartExpressionPreviewID = o.MultiPartExpressionPreviewID,
                QTIItemPreviewRequestID = o.QTIItemPreviewRequestID,
                Expression = o.Expression,
                EnableElements = o.EnableElements,
                Order = o.Order,
                Rules = o.Rules
            });
        }
    }
}
