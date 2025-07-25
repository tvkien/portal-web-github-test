using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TopicRepository : IRepository<Topic>, ITopicRepository
    {
        private readonly Table<TopicEntity> table;
        private readonly ItemTagDataContext dbContext;

        public TopicRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<TopicEntity>();
            Mapper.CreateMap<Topic, TopicEntity>();
            dbContext = ItemTagDataContext.Get(connectionString);
        }

        public IQueryable<Topic> Select()
        {
            return table.Select(x => new Topic
                {
                    TopicID = x.TopicID,
                    Name = x.Name
                });
        }
        public void Save(Topic item)
        {
            var entity = table.FirstOrDefault(x => x.TopicID.Equals(item.TopicID));

            if (entity.IsNull())
            {
                entity = new TopicEntity();
                table.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.TopicID = entity.TopicID;
        }

        public void Delete(Topic item)
        {
            var entity = table.FirstOrDefault(x => x.TopicID.Equals(item.TopicID));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public IList<string> GetTopicsBySearchText(string textToSearch, string inputIdString, string type)
        {
            return
                dbContext.GetTopicBySearchText(textToSearch, inputIdString, type).OrderBy(x => x.GroupTag).ThenBy(x => x.Name)
                    .Select(x => x.Name)
                    .ToList();
        }
    }
}