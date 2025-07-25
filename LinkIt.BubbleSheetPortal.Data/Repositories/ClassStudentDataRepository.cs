using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using System.Data.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using Envoc.Core.Shared.Extensions;
using AutoMapper;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ClassStudentDataRepository:IRepository<ClassStudentData>
    {
        private readonly Table<ClassStudentEntity> table;

        public ClassStudentDataRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<ClassStudentEntity>();
            Mapper.CreateMap<ClassStudentData, ClassStudentEntity>();
        }
                
        public IQueryable<ClassStudentData> Select()
        {
            return table.Select(x => new ClassStudentData {
                ClassStudentID = x.ClassStudentID,
                ClassID = x.ClassID,
                StudentID = x.StudentID,
                Active = x.Active,
                Code = x.Code,
                SISID = x.SISID,
                DistrictID = x.DistrictID
            });
        }

        public void Save(ClassStudentData item)
        {
            var entity = table.FirstOrDefault(x => x.ClassStudentID.Equals(item.ClassStudentID));

            if (entity.IsNull())
            {
                entity = new ClassStudentEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.ClassStudentID = entity.ClassStudentID;
        }

        public void Delete(ClassStudentData item)
        {
            var entity = table.FirstOrDefault(x => x.ClassStudentID.Equals(item.ClassStudentID));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}