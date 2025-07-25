using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using Envoc.Core.Shared.Extensions;
using AutoMapper;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StudentGradeHistoryRepository : IRepository<StudentGradeHistory>
    {
        private readonly Table<StudentGradeHistoryEntity> table;

        public StudentGradeHistoryRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<StudentGradeHistoryEntity>();
            Mapper.CreateMap<StudentGradeHistory, StudentGradeHistoryEntity>();
        }

        public IQueryable<StudentGradeHistory> Select()
        {
            return table.Select(x => new StudentGradeHistory
            {
                StudentGradeHistoryID = x.StudentGradeHistoryID,
                StudentID = x.StudentID,
                GradeID = x.GradeID,
                DateStart = x.DateStart,
                DateEnd = x.DateEnd
            });
        }

        public void Save(StudentGradeHistory item)
        {
            var entity = table.FirstOrDefault(x => x.StudentGradeHistoryID.Equals(item.StudentGradeHistoryID));

            if (entity.IsNull())
            {
                entity = new StudentGradeHistoryEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.StudentGradeHistoryID = entity.StudentGradeHistoryID;
        }

        public void Delete(StudentGradeHistory item)
        {
            var entity = table.FirstOrDefault(x => x.StudentGradeHistoryID.Equals(item.StudentGradeHistoryID));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
