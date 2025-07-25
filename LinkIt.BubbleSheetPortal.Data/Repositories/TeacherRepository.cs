using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TeacherRepository : IRepository<Teacher>
    {
        private readonly Table<TeacherEntity> table;

        public TeacherRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<TeacherEntity>();
            Mapper.CreateMap<Teacher, TeacherEntity>();
        }

        public IQueryable<Teacher> Select()
        {
            return table.Select(x => new Teacher
                {
                    Id = x.TeacherID,
                    Name = x.LastName,
                    UserId = x.UserID
                });
        }

        public void Save(Teacher item)
        {
            var entity = table.FirstOrDefault(x => x.TeacherID.Equals(item.Id));

            if(entity.IsNull())
            {
                entity = new TeacherEntity();
                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
            Mapper.Map(item, entity);
        }

        public void Delete(Teacher item)
        {
            var entity = table.FirstOrDefault(x => x.TeacherID.Equals(item.Id));

            if(!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}