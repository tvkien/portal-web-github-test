using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.InteractiveRubric.DataContext;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.Repositories
{
    public class RubricTestResultScoreRepository : IRubricTestResultScoreRepository
    {
        private readonly Table<RubricTestResultScoreEntity> table;

        public RubricTestResultScoreRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = RubricDataContext.Get(connectionString).GetTable<RubricTestResultScoreEntity>();
        }

        public void Delete(RubricTestResultScore item)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(IEnumerable<RubricTestResultScore> rubricTestResultScores)
        {
            var rubricTestResultScoreIds = rubricTestResultScores.Select(x => x.RubricTestResultScoreID).ToArray();
            var entities = table.Where(x => rubricTestResultScoreIds.Contains(x.RubricTestResultScoreID)).ToArray();
            if (entities?.Length > 0)
            {
                table.DeleteAllOnSubmit(entities);

                table.Context.SubmitChanges();
            }
        }

        public void Delete(IEnumerable<int> rubricTestResultScoresIds)
        {
            var entities = table.Where(x => rubricTestResultScoresIds.Contains(x.RubricTestResultScoreID)).ToArray();
            if (entities?.Length > 0)
            {
                table.DeleteAllOnSubmit(entities);

                table.Context.SubmitChanges();
            }
        }

        public void Save(RubricTestResultScore item)
        {
            throw new System.NotImplementedException();
        }

        public void SaveRubricTestResultScores(IEnumerable<RubricTestResultScoreDto> rubricTestResultScores, int qTIOnlineTestSessionId, int virtualQuestionId, int createdBy)
        {
            var rubricCategoriesIds = rubricTestResultScores.Select(x => x.RubricQuestionCategoryID).ToArray();
            var findRubricTestResultScores = table.Where(x => rubricCategoriesIds.Contains(x.RubricQuestionCategoryID) && x.VirtualQuestionID == virtualQuestionId && x.QTIOnlineTestSessionID == qTIOnlineTestSessionId).ToList();
            foreach (var item in rubricTestResultScores)
            {
                var entity = findRubricTestResultScores.FirstOrDefault(x => x.RubricQuestionCategoryID == item.RubricQuestionCategoryID);
                if (entity.IsNull())
                {
                    entity = new RubricTestResultScoreEntity();
                    entity.VirtualQuestionID = virtualQuestionId;
                    entity.QTIOnlineTestSessionID = qTIOnlineTestSessionId;
                    entity.RubricQuestionCategoryID = item.RubricQuestionCategoryID;
                    entity.Score = item.Score;
                    entity.CreatedDate = DateTime.UtcNow;
                    entity.CreatedBy = createdBy;
                    table.InsertOnSubmit(entity);
                }
                else
                {
                    entity.Score = item.Score;
                    entity.UpdatedBy = createdBy;
                    entity.UpdatedDate = DateTime.UtcNow;
                }
            }

            table.Context.SubmitChanges();
        }

        public IQueryable<RubricTestResultScore> Select()
        {
            return table.Select(x => new RubricTestResultScore
            {
                RubricQuestionCategoryID = x.RubricQuestionCategoryID,
                Score = x.Score,
                QTIOnlineTestSessionID = x.QTIOnlineTestSessionID,
                RubricTestResultScoreID = x.RubricTestResultScoreID,
                VirtualQuestionID = x.VirtualQuestionID,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate
            });
        }
    }
}
