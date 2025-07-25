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

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemoverLog
{
    public class TestResultSubScoreLogRepository : ITestResultSubScoreLogRepository
    {
        private readonly Table<TestResultSubScoreLogEntity> table;

        public TestResultSubScoreLogRepository(IConnectionString conn)
        {
            var connectionString = conn.GetAdminReportingLogConnectionString();
            table = TestResultLogDataContext.Get(connectionString).GetTable<TestResultSubScoreLogEntity>();
            Mapper.CreateMap<TestResultSubScoreLog, TestResultSubScoreLogEntity>();
        }

        public IQueryable<TestResultSubScoreLog> Select()
        {
            return table.Select(x => new TestResultSubScoreLog
            {
                TestResultSubScoreLogID = x.TestResultSubScoreLogID,
                TestResultSubScoreID = x.TestResultSubScoreID,
                TestResultScoreID = x.TestResultScoreID,
                ScorePercent = x.ScorePercent,
                ScorePercentage = x.ScorePercentage,
                ScoreRaw = x.ScoreRaw,
                ScoreScaled = x.ScoreScaled,
                ScoreLexile = x.ScoreLexile,
                UsePercent = x.UsePercent,
                UsePercentage = x.UsePercentage,
                UseRaw = x.UseRaw,
                UseScaled = x.UseScaled,
                UseLexile = x.UseLexile,
                PointsPossible = x.PointsPossible,
                AchievementLevel = x.AchievementLevel,
                UseGradeLevelEquiv = x.UseGradeLevelEquiv,
                ScoreGradeLevelEquiv = x.ScoreGradeLevelEquiv,
                Name = x.Name,
                MetStandard = x.MetStandard,
                ScoreCustomN_1 = x.ScoreCustomN_1,
                ScoreCustomN_2 = x.ScoreCustomN_2,
                ScoreCustomN_3 = x.ScoreCustomN_3,
                ScoreCustomN_4 = x.ScoreCustomN_4,
                ScoreCustomA_1 = x.ScoreCustomA_1,
                ScoreCustomA_2 = x.ScoreCustomA_2,
                ScoreCustomA_3 = x.ScoreCustomA_3,
                ScoreCustomA_4 = x.ScoreCustomA_4
            });
        }

        public void Save(TestResultSubScoreLog item)
        {
            try
            {
                var entity = table.FirstOrDefault(x => x.TestResultSubScoreLogID.Equals(item.TestResultSubScoreLogID));

                if (entity.IsNull())
                {
                    entity = new TestResultSubScoreLogEntity();
                    table.InsertOnSubmit(entity);
                }

                Mapper.Map(item, entity);
                table.Context.SubmitChanges();
                item.TestResultSubScoreLogID = entity.TestResultSubScoreLogID;
            }
            catch (Exception ex) { }
        }
        public void Save(IList<TestResultSubScoreLog> testResultSubScoreLogs)
        {
            try
            {
                foreach (var item in testResultSubScoreLogs)
                {
                    TestResultSubScoreLogEntity entity;
                    if (item.TestResultSubScoreLogID.Equals(0))
                    {
                        entity = new TestResultSubScoreLogEntity();
                        table.InsertOnSubmit(entity);
                    }
                    else
                    {
                        entity = table.FirstOrDefault(x => x.TestResultSubScoreLogID.Equals(item.TestResultSubScoreLogID));
                    }
                    if (entity != null)
                    {
                        Mapper.Map(item, entity);
                    }
                }
                table.Context.SubmitChanges();
            }
            catch (Exception ex) { }
        }
        public void Delete(TestResultSubScoreLog item)
        {
            throw new NotImplementedException();
        }       
    }
}
