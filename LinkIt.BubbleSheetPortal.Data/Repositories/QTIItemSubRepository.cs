using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIItemSubRepository : IReadOnlyRepository<QTIItemSub>, IInsertDeleteRepository<QTIItemSub>
    {
        private readonly Table<QTIItemSubEntity> table;

        public QTIItemSubRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = AssessmentDataContext.Get(connectionString);
            table = dataContext.GetTable<QTIItemSubEntity>();
        }

        public IQueryable<QTIItemSub> Select()
        {
            return table.Select(x => new QTIItemSub
                                  {
                                      CorrectAnswer = x.CorrectAnswer,
                                      PointsPossible = x.PointsPossible,
                                      QTIItemId = x.QTIItemID,
                                      QTIItemSubId = x.QTIItemSubID,
                                      QTISchemaId = x.QTISchemaID,
                                      ResponseIdentifier = x.ResponseIdentifier,
                                      ResponseProcessing = x.ResponseProcessing,
                                      ResponseProcessingTypeId = x.ResponseProcessingTypeID,
                                      SourceId = x.SourceID,
                                      Updated = x.Updated,
                                      Depending = x.Depending,
                                      Major = x.Major
                                  });
            
        }

        public void Save(QTIItemSub item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemSubID.Equals(item.QTIItemSubId));

            if (entity == null)
            {
                entity = new QTIItemSubEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            table.Context.SubmitChanges();
            item.QTIItemSubId = entity.QTIItemSubID;
        }

        public void Delete(QTIItemSub item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemSubID.Equals(item.QTIItemSubId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }       

        private void MapModelToEntity(QTIItemSubEntity entity, QTIItemSub item)
        {
            entity.CorrectAnswer = item.CorrectAnswer;
            entity.PointsPossible = item.PointsPossible;
            entity.QTIItemID = item.QTIItemId;
            entity.QTIItemSubID = item.QTIItemSubId;
            entity.QTISchemaID = item.QTISchemaId;
            entity.ResponseIdentifier = item.ResponseIdentifier;
            entity.ResponseProcessing = item.ResponseProcessing;
            entity.ResponseProcessingTypeID = item.ResponseProcessingTypeId;
            entity.SourceID = item.SourceId;
            entity.Updated = item.Updated;
            entity.Major = item.Major;
            entity.Depending = item.Depending;
        }

        public void InsertMultipleRecord(List<QTIItemSub> items)
        {
            foreach (var item in items)
            {
                var entity = new QTIItemSubEntity();
                MapModelToEntity(entity, item);
                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
