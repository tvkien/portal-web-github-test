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
    public class LessonOneRepository : IRepository<LessonOne>, ILessonOneRepository
    {
        private readonly Table<LessonOneEntity> table;
        private readonly ItemTagDataContext dbContext;

        public LessonOneRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<LessonOneEntity>();
            Mapper.CreateMap<LessonOne, LessonOneEntity>();
            dbContext = ItemTagDataContext.Get(connectionString);
        }

        public IQueryable<LessonOne> Select()
        {
            return table.Select(x => new LessonOne
                {
                    LessonOneID = x.LessonOneID,
                    Name = x.Name
                });
        }
        public void Save(LessonOne item)
        {
            var entity = table.FirstOrDefault(x => x.LessonOneID.Equals(item.LessonOneID));

            if (entity.IsNull())
            {
                entity = new LessonOneEntity();
                table.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.LessonOneID = entity.LessonOneID;
        }

        public void Delete(LessonOne item)
        {
            var entity = table.FirstOrDefault(x => x.LessonOneID.Equals(item.LessonOneID));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public IList<string> GetLessonOnesBySearchText(string textToSearch, string inputIdString, string type)
        {
            return
                dbContext.GetLessonOneBySearchText(textToSearch, inputIdString, type).OrderBy(x => x.GroupTag).ThenBy(x => x.Name)
                    .Select(x => x.Name)
                    .ToList();
        }
    }
}