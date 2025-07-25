using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using Envoc.Core.Shared.Extensions;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models;
namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemoverLog
{
    public class TestResultProgramLogRepository : ITestResultProgramLogRepository
    {
        private readonly Table<TestResultProgramLogEntity> table;

        public TestResultProgramLogRepository(IConnectionString conn)
        {
            var connectionString = conn.GetAdminReportingLogConnectionString();
            table = TestResultLogDataContext.Get(connectionString).GetTable<TestResultProgramLogEntity>();
            Mapper.CreateMap<TestResultProgramLog, TestResultProgramLogEntity>();
        }

        public IQueryable<TestResultProgramLog> Select()
        {
            return table.Select(x => new TestResultProgramLog
                    {
                        TestResultProgramLogID = x.TestResultProgramLogID,
                        TestResultProgramID = x.TestResultProgramID,
                        TestResultID = x.TestResultID,
                        ProgramID = x.ProgramID
                    });
        }

        public void Save(TestResultProgramLog item)
        {
            try
            {
                var entity = table.FirstOrDefault(x => x.TestResultProgramLogID.Equals(item.TestResultProgramLogID));

                if (entity == null)
                {
                    entity = new TestResultProgramLogEntity();
                    table.InsertOnSubmit(entity);
                }

                Mapper.Map(item, entity);
                table.Context.SubmitChanges();
                item.TestResultProgramLogID = entity.TestResultProgramLogID;
            }
            catch (Exception ex) { }
        }
        public void Save(IList<TestResultProgramLog> testResultProgramLogs)
        {
            try
            {
                foreach (var item in testResultProgramLogs)
                {
                    TestResultProgramLogEntity entity;
                    if (item.TestResultProgramLogID.Equals(0))
                    {
                        entity = new TestResultProgramLogEntity();
                        table.InsertOnSubmit(entity);
                    }
                    else
                    {
                        entity = table.FirstOrDefault(x => x.TestResultProgramLogID.Equals(item.TestResultProgramLogID));
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
        public void Delete(TestResultProgramLog item)
        {
            if (item.IsNotNull())
            {
                throw new NotImplementedException();
            }
        }
    }
}
