using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualQuestionTopicRepository : IVirtualQuestionTopicRepository
    {
        private readonly Table<VirtualQuestionTopicEntity> table;
        private readonly Table<VirtualQuestionTopicView> view;

        public VirtualQuestionTopicRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<VirtualQuestionTopicEntity>();
            view = dataContext.GetTable<VirtualQuestionTopicView>();
        }

        public IQueryable<VirtualQuestionTopic> Select()
        {
            return view.Select(x => new VirtualQuestionTopic
                {
                    VirtualQuestionTopicId = x.VirtualQuestionTopicID,
                    VirtualQuestionId = x.VirtualQuestionID,
                    VirtualTestId = x.VirtualTestID,
                    TopicId = x.TopicId,
                    Name = x.Name
                });
        }

        public void Save(VirtualQuestionTopic item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionTopicID.Equals(item.VirtualQuestionTopicId));

            if (entity == null)
            {
                entity = new VirtualQuestionTopicEntity();
                table.InsertOnSubmit(entity);
            }
            entity.VirtualQuestionID = item.VirtualQuestionId;
            entity.TopicID = item.TopicId;

            table.Context.SubmitChanges();
            item.VirtualQuestionTopicId = entity.VirtualQuestionTopicID;
        }

        public void Delete(VirtualQuestionTopic item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionTopicID.Equals(item.VirtualQuestionTopicId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public void InsertMultipleRecord(List<VirtualQuestionTopic> items)
        {
            foreach (var item in items)
            {
                var entity = new VirtualQuestionTopicEntity
                {
                    VirtualQuestionID = item.VirtualQuestionId,
                    TopicID = item.TopicId
                };

                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
