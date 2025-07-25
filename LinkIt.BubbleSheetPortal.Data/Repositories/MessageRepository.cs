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
    public class MessageRepository : IReadOnlyRepository<Message>, IInsertDeleteRepository<Message>
    {
        private readonly Table<MessageEntity> table;
        private readonly ParentDataContext _parentDataContext;

        public MessageRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = ParentDataContext.Get(connectionString).GetTable<MessageEntity>();
            _parentDataContext = ParentDataContext.Get(connectionString);
        }

        #region Implementation of IReadOnlyRepository

        public IQueryable<Message> Select()
        {
            return table.Select(x => new Message
            {
                 Body = x.Body,
                 CreatedDateTime = x.CreatedDateTime.Value,
                 IsAcknowlegdeRequired = x.IsAcknowlegdeRequired,
                 IsDeleted = x.IsDeleted,
                 MessageId = x.MessageID,
                 MessageType = x.MessageType,
                 MessageRef = x.MessageRef.Value,
                 Subject = x.Subject,
                 UserId = x.UserID,
                 ReplyEnabled = x.ReplyEnabled
            });
        }

        #endregion
        
        #region Implementation of IInsertDeleteRepository

        public void Save(Message item)
        {
            var entity = table.FirstOrDefault(x => x.MessageID.Equals(item.MessageId));
            if (entity.IsNull())
            {
                entity = new MessageEntity();
                table.InsertOnSubmit(entity);
            }
            BindItem(entity, item);
            table.Context.SubmitChanges();
            item.MessageId = entity.MessageID;
        }

        public void Delete(Message item)
        {
            var entity = table.FirstOrDefault(x => x.MessageID.Equals(item.MessageId));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        #endregion

        private void BindItem(MessageEntity entity, Message item)
        {
            if (entity.MessageID == 0)
            {
                if (item.CreatedDateTime == DateTime.MinValue)
                    entity.CreatedDateTime = DateTime.Now;
                else
                    entity.CreatedDateTime = item.CreatedDateTime;
            }

            entity.Body = item.Body;
            entity.IsAcknowlegdeRequired = item.IsAcknowlegdeRequired;
            entity.IsDeleted = item.IsDeleted;
            entity.MessageType = item.MessageType;
            entity.MessageRef = item.MessageRef;
            entity.Subject = item.Subject;
            entity.UserID = item.UserId;
            entity.ReplyEnabled = item.ReplyEnabled;
        }

        public void InsertMultipleRecord(List<Message> items)
        {
            foreach (var item in items)
            {
                var entity = new MessageEntity();
                BindItem(entity, item);
                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
