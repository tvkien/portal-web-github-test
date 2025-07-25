using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ClassStudentReadOnlyRepository : IReadOnlyRepository<ClassStudent>
    {
        private readonly Table<ClassStudentView> table;

        public ClassStudentReadOnlyRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<ClassStudentView>();
            Mapper.CreateMap<ClassStudent, ClassStudentView>();
        }

        public IQueryable<ClassStudent> Select()
        {
            return table.Select(x => new ClassStudent
                {
                    ClassId = x.ClassID,
                    StudentId = x.StudentID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Active = x.Active,
                    Code = x.Code
                });
        }
    }
}