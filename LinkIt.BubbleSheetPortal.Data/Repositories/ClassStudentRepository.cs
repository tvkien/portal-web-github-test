using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ClassStudentRepository : IRepository<ClassStudent>
    {
        private readonly Table<ClassStudentEntity> table;

        public ClassStudentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<ClassStudentEntity>();
            Mapper.CreateMap<ClassStudent, ClassStudentEntity>();
        }

        public IQueryable<ClassStudent> Select()
        {
            return table.Select(x => new ClassStudent
            {
                Id = x.ClassStudentID,
                ClassId = x.ClassID,
                StudentId = x.StudentID,
                Active = x.Active,
                Code = x.Code,                
            });
        }

        public void Save(ClassStudent item)
        {
            var entity = table.FirstOrDefault(x => x.ClassStudentID.Equals(item.Id));

            if (entity == null)
            {
                entity = new ClassStudentEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.Id = entity.ClassStudentID;
        }

        public void Delete(ClassStudent item)
        {
            var entity = table.FirstOrDefault(x => x.ClassStudentID.Equals(item.Id));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}