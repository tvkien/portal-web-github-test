using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using AutoMapper;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TestFeedbackRepository : IRepository<TestFeedback>
    {
        private readonly Table<TestFeedbackEntity> table;
        private readonly TestDataContext dbContext;

        public TestFeedbackRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = TestDataContext.Get(connectionString);
            table = dbContext.GetTable<TestFeedbackEntity>();
        }

        public IQueryable<TestFeedback> Select()
        {
            return table.Select(x => new TestFeedback
                                         {
                                             TestFeedbackID = x.TestFeedbackID,
                                             QtiOnlineTestSessionID = x.QtiOnlineTestSessionID,
                                             TestResultID = x.TestResultID,
                                             Feedback = x.Feedback,
                                             UserID = x.UserID,
                                             UpdatedDate = x.UpdatedDate
                                         });
        }

        public void Save(TestFeedback item)
        {
            var entity = table.FirstOrDefault(x => x.TestFeedbackID.Equals(item.TestFeedbackID));

            if (entity.IsNull())
            {
                entity = new TestFeedbackEntity();
                table.InsertOnSubmit(entity);
            }
            entity.QtiOnlineTestSessionID = item.QtiOnlineTestSessionID;
            entity.TestResultID = item.TestResultID;
            entity.Feedback = item.Feedback;
            entity.UserID = item.UserID;
            entity.UpdatedDate = item.UpdatedDate;
            
            table.Context.SubmitChanges();
            item.TestFeedbackID = entity.TestFeedbackID;
        }

        public void Delete(TestFeedback item)
        {
            var entity = table.FirstOrDefault(x => x.TestFeedbackID.Equals(item.TestFeedbackID));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}