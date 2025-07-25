using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualQuestionAnswerScoreRepository : IRepository<VirtualQuestionAnswerScore>
    {
        private readonly Table<VirtualQuestionAnswerScoreEntity> table;

        public VirtualQuestionAnswerScoreRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<VirtualQuestionAnswerScoreEntity>();
        }

        public IQueryable<VirtualQuestionAnswerScore> Select()
        {
            return table.Select(x => new VirtualQuestionAnswerScore
                {
                    QTIItemAnswerScoreId = x.QTIItemAnswerScoreID,
                    Score = x.Score,
                    VirtualQuestionAnswerScoreId = x.VirtualQuestionAnswerScoreID,
                    VirtualQuestionId = x.VirtualQuestionID
                });
        }

        public void Save(VirtualQuestionAnswerScore item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionAnswerScoreID.Equals(item.VirtualQuestionAnswerScoreId));

            if (entity == null)
            {
                entity = new VirtualQuestionAnswerScoreEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            table.Context.SubmitChanges();
            item.VirtualQuestionAnswerScoreId = entity.VirtualQuestionAnswerScoreID;
        }

        public void Delete(VirtualQuestionAnswerScore item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionAnswerScoreID.Equals(item.VirtualQuestionAnswerScoreId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(VirtualQuestionAnswerScoreEntity entity, VirtualQuestionAnswerScore item)
        {
            entity.QTIItemAnswerScoreID = item.QTIItemAnswerScoreId;
            entity.VirtualQuestionID = item.VirtualQuestionId;
            entity.Score = item.Score;
            entity.VirtualQuestionAnswerScoreID = item.VirtualQuestionAnswerScoreId;
        }
    }
}