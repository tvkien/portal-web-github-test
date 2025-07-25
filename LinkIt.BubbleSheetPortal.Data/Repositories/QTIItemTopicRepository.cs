using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIItemTopicRepository  : IRepository<QTIItemTopic>
    {
        private readonly Table<QTIItemTopicEntity> table;
        private readonly Table<QTIItemTopicView> view;

        public QTIItemTopicRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTIItemTopicEntity>();
            view = AssessmentDataContext.Get(connectionString).GetTable<QTIItemTopicView>();
            Mapper.CreateMap<QTIItemTopic, QTIItemTopicEntity>();
        }

        public IQueryable<QTIItemTopic> Select()
        {
            return view.Select(x => new QTIItemTopic
                {
                    QTIItemTopicID = x.QTIItemTopicID,
                    QTIItemID = x.QTIItemID,
                    TopicId = x.TopicId,
                    Name = x.Name
                });
        }
        public void Save(QTIItemTopic item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemTopicID.Equals(item.QTIItemTopicID));

            if (entity.IsNull())
            {
                entity = new QTIItemTopicEntity();
                table.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.QTIItemTopicID = entity.QTIItemTopicID;
        }

        public void Delete(QTIItemTopic item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemTopicID.Equals(item.QTIItemTopicID));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}