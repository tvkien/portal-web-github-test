using System.Linq;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StudentMetaRepository : IRepository<StudentMeta>
    {
        private readonly Table<StudentMetaEntity> studentMetaEntityTable;

        public StudentMetaRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var datacontext = StudentDataContext.Get(connectionString);
            studentMetaEntityTable = datacontext.GetTable<StudentMetaEntity>();
            Mapper.CreateMap<StudentMeta, StudentMetaEntity>();
        }

        public IQueryable<StudentMeta> Select()
        {
            return studentMetaEntityTable.Select(x => new StudentMeta
                {
                    StudentMetaID = x.StudentMetaID,
                    StudentID = x.StudentID,
                    Data = x.Data,
                    Code = x.Code,
                    Name = x.Name
                });
        }

        public void Save(StudentMeta item)
        {
            var entity = studentMetaEntityTable.FirstOrDefault(x => x.StudentMetaID.Equals(item.StudentMetaID));

            if (entity.IsNull())
            {
                entity = new StudentMetaEntity();
                studentMetaEntityTable.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            studentMetaEntityTable.Context.SubmitChanges();
        }

        public void Delete(StudentMeta item)
        {
            var entity = studentMetaEntityTable.FirstOrDefault(x => x.StudentMetaID.Equals(item.StudentMetaID));
            if (entity != null)
            {
                studentMetaEntityTable.DeleteOnSubmit(entity);
                studentMetaEntityTable.Context.SubmitChanges();
            }
        }       
    }
}