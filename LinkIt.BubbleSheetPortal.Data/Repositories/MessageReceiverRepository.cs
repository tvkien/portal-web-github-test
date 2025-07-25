using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class MessageReceiverRepository : IReadOnlyRepository<MessageReceiver>, IInsertDeleteRepository<MessageReceiver>
    {
        private readonly Table<MessageReceiverEntity> table;
        private readonly ParentDataContext _parentDataContext;

        public MessageReceiverRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = ParentDataContext.Get(connectionString).GetTable<MessageReceiverEntity>();
            _parentDataContext = ParentDataContext.Get(connectionString);
        }

        #region Implementation of IReadOnlyRepository

        public IQueryable<MessageReceiver> Select()
        {
            return table.Select(x => new MessageReceiver
            {
                 IsDeleted = x.IsDeleted,
                 MessageId = x.MessageID,
                 IsRead = x.IsRead,
                 MessageReceiverId = x.MessageReceiverID,
                 StudentId = x.StudentID,
                 UserId = x.UserID
            });
        }

        #endregion
        
        #region Implementation of IInsertDeleteRepository

        public void Save(MessageReceiver item)
        {
            var entity = table.FirstOrDefault(x => x.MessageReceiverID.Equals(item.MessageReceiverId));
            if (entity.IsNull())
            {
                entity = new MessageReceiverEntity();
                table.InsertOnSubmit(entity);
            }
            BindItem(entity, item);
            table.Context.SubmitChanges();
            item.MessageReceiverId = entity.MessageReceiverID;
        }

        public void Delete(MessageReceiver item)
        {
            var entity = table.FirstOrDefault(x => x.MessageReceiverID.Equals(item.MessageReceiverId));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        #endregion

        private void BindItem(MessageReceiverEntity entity, MessageReceiver item)
        {
            entity.IsDeleted = item.IsDeleted;
            entity.IsRead = item.IsRead;
            entity.MessageID = item.MessageId;
            entity.StudentID = item.StudentId;
            entity.UserID = item.UserId;
        }

        public void InsertMultipleRecord(List<MessageReceiver> items)
        {
            foreach (var item in items)
            {
                var entity = new MessageReceiverEntity();
                BindItem(entity, item);
                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
