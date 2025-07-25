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
    public class TestResultAuditRepository : ITestResultAuditRepository
    {
        private readonly Table<TestResultAuditEntity> table;
        private readonly Table<TestResultRemoverLogEntity> testResultRemoverLogTable;

        public TestResultAuditRepository(IConnectionString conn)
        {
            var connectionString = conn.GetAdminReportingLogConnectionString();
            table = TestResultLogDataContext.Get(connectionString).GetTable<TestResultAuditEntity>();
            testResultRemoverLogTable = TestResultLogDataContext.Get(connectionString).GetTable<TestResultRemoverLogEntity>();
            Mapper.CreateMap<TestResultAudit, TestResultAuditEntity>();
        }

        public IQueryable<TestResultAudit> Select()
        {
            return table.Select(x => new TestResultAudit
                    {
                       TestResultAuditId = x.TestResultAuditID,
                       TestResultId = x.TestResultID,
                       AuditDate = x.AuditDate,
                       UserId = x.UserID,
                       Type = x.Type,
                       IPAddress = x.IPAddress
                    });
        }

        public void Save(TestResultAudit item)
        {
            try
            {
                var entity = table.FirstOrDefault(x => x.TestResultAuditID.Equals(item.TestResultAuditId));

                if (entity == null)
                {
                    entity = new TestResultAuditEntity();
                    table.InsertOnSubmit(entity);
                }

                Mapper.Map(item, entity);
                table.Context.SubmitChanges();
                item.TestResultAuditId = entity.TestResultAuditID;
            }
            catch (Exception ex) { }
        }

        public void Save(IList<TestResultAudit> testResultAudits)
        {
            try
            {
                foreach (var item in testResultAudits)
                {
                    TestResultAuditEntity entity;
                    if (item.TestResultAuditId.Equals(0))
                    {
                        entity = new TestResultAuditEntity();
                        table.InsertOnSubmit(entity);
                    }
                    else
                    {
                        entity = table.FirstOrDefault(x => x.TestResultAuditID.Equals(item.TestResultAuditId));
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
        public void Delete(TestResultAudit item)
        {
            if (item.IsNotNull())
            {
                throw new NotImplementedException();
            }
        }

        public void SaveTestResultRemoverLog(TestResultAudit testResultAudit)
        {
            try
            {
                var newEntity = new TestResultRemoverLogEntity
                {
                    ActionType = testResultAudit.Type,
                    CreatedDate = DateTime.UtcNow,
                    IPAddress = testResultAudit.IPAddress,
                    TestResultIDs = testResultAudit.TestResultIDs,
                    UserID = testResultAudit.UserId
                };

                testResultRemoverLogTable.InsertOnSubmit(newEntity);
                testResultRemoverLogTable.Context.SubmitChanges();
            }
            catch (Exception ex) { }
        }
    }
}
