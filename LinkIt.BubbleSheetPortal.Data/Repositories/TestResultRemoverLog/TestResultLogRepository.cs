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
    public class TestResultLogRepository : ITestResultLogRepository
    {
        private readonly Table<TestResultLogEntity> table;

        public TestResultLogRepository(IConnectionString conn)
        {
            var connectionString = conn.GetAdminReportingLogConnectionString();
            table = TestResultLogDataContext.Get(connectionString).GetTable<TestResultLogEntity>();
            Mapper.CreateMap<TestResultLog, TestResultLogEntity>();
        }

        public IQueryable<TestResultLog> Select()
        {
            return table.Select(x => new TestResultLog
            {
                TestResultLogID = x.TestResultLogID,
                TestResultID = x.TestResultID,
                VirtualTestID = x.VirtualTestID,
                StudentID = x.StudentID,
                TeacherID = x.TeacherID,
                SchoolID = x.SchoolID,
                ResultDate = x.ResultDate,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                TermID = x.TermID,
                ClassID = x.ClassID,
                GradedByID = x.GradedByID,
                ScoreType = x.ScoreType,
                ScoreValue = x.ScoreValue,
                SubmitType = x.SubmitType,
                DistrictTermID = x.DistrictTermID,
                UserID = x.UserID,
                OriginalUserID = x.OriginalUserID,
                LegacyBatchID = x.LegacyBatchID,
                BubbleSheetID = x.BubbleSheetID,
                QTIOnlineTestSessionID = x.QTIOnlineTestSessionID,
                DistrictID = x.DistrictID,
                DistrictName = x.DistrictName,                
                DistrictTermName = x.DistrictTermName,                
                SchoolName = x.SchoolName,                
                UserName = x.UserName,
                ClassName = x.ClassName,
                StudentFirst = x.StudentFirst,
                StudentLast = x.StudentLast,                
                TestName = x.TestName
            });
        }

        public void Save(TestResultLog item)
        {
            try
            {
                var entity = table.FirstOrDefault(x => x.TestResultLogID.Equals(item.TestResultLogID));

                if (entity.IsNull())
                {
                    entity = new TestResultLogEntity();
                    table.InsertOnSubmit(entity);
                }

                Mapper.Map(item, entity);
                table.Context.SubmitChanges();
                item.TestResultLogID = entity.TestResultLogID;
            }
            catch (Exception ex) { }
        }

        public void Save(IList<TestResultLog> testResultLogs)
        {
            try
            {
                foreach (var item in testResultLogs)
                {
                    TestResultLogEntity entity;
                    if (item.TestResultLogID.Equals(0))
                    {
                        entity = new TestResultLogEntity();
                        table.InsertOnSubmit(entity);
                    }
                    else
                    {
                        entity = table.FirstOrDefault(x => x.TestResultLogID.Equals(item.TestResultLogID));
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
        public void Delete(TestResultLog item)
        {
            throw new NotImplementedException();
        }       
    }
}
