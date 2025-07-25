using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemoverLog
{
    public class TestResultScoreLogRepository : ITestResultScoreLogRepository
    {
        private readonly Table<TestResultScoreLogEntity> table;

        public TestResultScoreLogRepository(IConnectionString conn)
        {
            var connectionString = conn.GetAdminReportingLogConnectionString();
            table = TestResultLogDataContext.Get(connectionString).GetTable<TestResultScoreLogEntity>();
            Mapper.CreateMap<TestResultScoreLog, TestResultScoreLogEntity>();
        }

        public IQueryable<TestResultScoreLog> Select()
        {
            return table.Select(x => new TestResultScoreLog
            {
                TestResultScoreLogID = x.TestResultScoreLogID,
                TestResultScoreID = x.TestResultScoreID,
                TestResultID = x.TestResultID,
                TookTest = x.TookTest,
                ScorePercent = x.ScorePercent,
                ScorePercentage = x.ScorePercentage,
                ScoreRaw = x.ScoreRaw,
                ScoreScaled = x.ScoreScaled,
                ScoreLexile = x.ScoreLexile,
                ScoreIndex = x.ScoreIndex,
                UsePercent = x.UsePercent,
                UsePercentage = x.UsePercentage,
                UseRaw = x.UseRaw,
                UseScaled = x.UseScaled,
                UseLexile = x.UseLexile,
                UseIndex = x.UseIndex,
                PointsPossible = x.PointsPossible,
                AchievementLevel = x.AchievementLevel,
                UseGradeLevelEquiv = x.UseGradeLevelEquiv,
                ScoreGradeLevelEquiv = x.ScoreGradeLevelEquiv,
                Name = x.Name,
                MetStandard = x.MetStandard,
                AchieveLevelName = x.AchieveLevelName,
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

        public void Save(TestResultScoreLog item)
        {
            try
            {
                var entity = table.FirstOrDefault(x => x.TestResultScoreLogID.Equals(item.TestResultScoreLogID));

                if (entity.IsNull())
                {
                    entity = new TestResultScoreLogEntity();
                    table.InsertOnSubmit(entity);
                }

                Mapper.Map(item, entity);
                table.Context.SubmitChanges();
                item.TestResultScoreLogID = entity.TestResultScoreLogID;
            }
            catch (Exception ex) { }
        }
        public void Save(IList<TestResultScoreLog> testResultScoreLogs)
        {
            try
            {
                foreach (var item in testResultScoreLogs)
                {
                    TestResultScoreLogEntity entity;
                    if (item.TestResultScoreLogID.Equals(0))
                    {
                        entity = new TestResultScoreLogEntity();
                        table.InsertOnSubmit(entity);
                    }
                    else
                    {
                        entity = table.FirstOrDefault(x => x.TestResultScoreLogID.Equals(item.TestResultScoreLogID));
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
        public void Delete(TestResultScoreLog item)
        {
            throw new NotImplementedException();
        }       
    }
}
