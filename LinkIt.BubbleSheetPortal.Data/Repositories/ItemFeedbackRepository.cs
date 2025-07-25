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
    public class ItemFeedbackRepository : IRepository<ItemFeedback>
    {
        private readonly Table<ItemFeedbackEntity> table;
        private readonly TestDataContext dbContext;

        public ItemFeedbackRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = TestDataContext.Get(connectionString);
            table = dbContext.GetTable<ItemFeedbackEntity>();
        }

        public IQueryable<ItemFeedback> Select()
        {
            return table.Select(x => new ItemFeedback
                                         {
                                             ItemFeedbackID = x.ItemFeedbackID,
                                             QTIOnlineTestSessionAnswerID = x.QTIOnlineTestSessionAnswerID,
                                             AnswerID = x.AnswerID,
                                             Feedback = x.Feedback,
                                             UserID = x.UserID,
                                             UpdatedDate = x.UpdatedDate,
                                             DocumentGUID = x.DocumentGUID
                                         });
        }

        public void Save(ItemFeedback item)
        {
            var entity = table.FirstOrDefault(x => x.ItemFeedbackID.Equals(item.ItemFeedbackID));

            if (entity.IsNull())
            {
                entity = new ItemFeedbackEntity();
                table.InsertOnSubmit(entity);
            }
            if (item.AnswerID > 0)
            {
                entity.QTIOnlineTestSessionAnswerID = 0;
                entity.AnswerID = item.AnswerID;
            }
            else
            {
                entity.QTIOnlineTestSessionAnswerID = item.QTIOnlineTestSessionAnswerID;
                entity.AnswerID = 0;
            }

            entity.Feedback = item.Feedback;
            entity.UserID = item.UserID;
            entity.UpdatedDate = item.UpdatedDate;
            entity.DocumentGUID = item.DocumentGUID;
            
            table.Context.SubmitChanges();
            item.ItemFeedbackID = entity.ItemFeedbackID;
        }

        public void Delete(ItemFeedback item)
        {
            var entity = table.FirstOrDefault(x => x.ItemFeedbackID.Equals(item.ItemFeedbackID));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
