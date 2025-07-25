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
    public class LessonTwoRepository : IRepository<LessonTwo>, ILessonTwoRepository
    {
        private readonly Table<LessonTwoEntity> table;
        private readonly ItemTagDataContext dbContext;

        public LessonTwoRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<LessonTwoEntity>();
            Mapper.CreateMap<LessonTwo, LessonTwoEntity>();
            dbContext = ItemTagDataContext.Get(connectionString);
        }

        public IQueryable<LessonTwo> Select()
        {
            return table.Select(x => new LessonTwo
                {
                    LessonTwoID = x.LessonTwoID,
                    Name = x.Name
                });
        }
        public void Save(LessonTwo item)
        {
            var entity = table.FirstOrDefault(x => x.LessonTwoID.Equals(item.LessonTwoID));

            if (entity.IsNull())
            {
                entity = new LessonTwoEntity();
                table.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.LessonTwoID = entity.LessonTwoID;
        }

        public void Delete(LessonTwo item)
        {
            var entity = table.FirstOrDefault(x => x.LessonTwoID.Equals(item.LessonTwoID));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public IList<string> GetLessonTwosBySearchText(string textToSearch, string inputIdString, string type)
        {
            var data =
                dbContext.GetLessonTwoBySearchText(textToSearch, inputIdString, type).OrderBy(x => x.GroupTag).ThenBy(x => x.Name)
                    .Select(x => x.Name)
                    .ToList();            
            return data;
        }
    }
}