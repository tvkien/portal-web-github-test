using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Threading;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TestResultScoreRepository : IReadOnlyRepository<TestResultScore>
    {
        private readonly Table<TestResultScoreEntity> table;
        private readonly TestDataContext dbContext;

        public TestResultScoreRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<TestResultScoreEntity>();
            dbContext = TestDataContext.Get(connectionString);
        }

        public IQueryable<TestResultScore> Select()
        {
            return table.Select(x => new TestResultScore
            {
                TestResultScoreID = x.TestResultScoreID,
                TestResultID = x.TestResultID,
                PointsPossible = x.PointsPossible,
                ScoreLexile = x.ScoreLexile,
                ScoreRaw = x.ScoreRaw,
                ScoreScaled = x.ScoreScaled,
                ScoreIndex = x.ScoreIndex,
                ScorePercent = x.ScorePercent,
                ScorePercentage = x.ScorePercentage,
                AchievementLevel = x.AchievementLevel
            });
        }

        public void Save(TestResultScore item)
        {
            var entity = table.FirstOrDefault(x => x.TestResultID.Equals(item.TestResultScoreID));

            if (entity.IsNull())
            {
                entity = new TestResultScoreEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.TestResultScoreID = entity.TestResultScoreID;
        }
    }
}
