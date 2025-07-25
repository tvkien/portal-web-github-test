using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIItemAnswerScoreRepository : IReadOnlyRepository<QTIItemAnswerScore>, IInsertDeleteRepository<QTIItemAnswerScore>
    {
        private readonly Table<QTIItemAnswerScoreEntity> table;

        public QTIItemAnswerScoreRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = AssessmentDataContext.Get(connectionString);
            table = dataContext.GetTable<QTIItemAnswerScoreEntity>();
        }

        public IQueryable<QTIItemAnswerScore> Select()
        {
            return table.Select(x => new QTIItemAnswerScore
                                  {
                                      Answer = x.Answer,
                                      QTIItemAnswerScoreId = x.QTIItemAnswerScoreID,
                                      QTIItemId = x.QTIItemID,
                                      ResponseIdentifier = x.ResponseIdentifier,
                                      Score = x.Score,
                                      AnswerText = x.AnswerText
            });
            
        }

        public void Save(QTIItemAnswerScore item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemAnswerScoreID.Equals(item.QTIItemAnswerScoreId));

            if (entity == null)
            {
                entity = new QTIItemAnswerScoreEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            table.Context.SubmitChanges();
            item.QTIItemAnswerScoreId = entity.QTIItemAnswerScoreID;
        }

        public void Delete(QTIItemAnswerScore item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemAnswerScoreID.Equals(item.QTIItemAnswerScoreId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(QTIItemAnswerScoreEntity entity, QTIItemAnswerScore item)
        {
            entity.Answer = item.Answer;
            entity.QTIItemAnswerScoreID = item.QTIItemAnswerScoreId;
            entity.QTIItemID = item.QTIItemId;
            entity.ResponseIdentifier = item.ResponseIdentifier;
            entity.Score = item.Score;
            entity.AnswerText = item.AnswerText;
        }

        public void InsertMultipleRecord(List<QTIItemAnswerScore> items)
        {
            foreach (var item in items)
            {
                var entity = new QTIItemAnswerScoreEntity();
                MapModelToEntity(entity, item);
                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
