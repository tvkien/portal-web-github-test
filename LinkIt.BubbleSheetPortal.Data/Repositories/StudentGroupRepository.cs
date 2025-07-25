using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.GroupPrinting;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StudentGroupRepository : IReadOnlyRepository<StudentGroup>
    {
        private readonly Table<StudentGroupView> table;

        public StudentGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<StudentGroupView>();
        }

        public IQueryable<StudentGroup> Select()
        {
            return table.Select(x => new StudentGroup
                                         {
                                             ClassId = x.ClassID,
                                             ClassName = x.ClassName,
                                             Code = x.Code,
                                             DistrictName = x.DistrictName,
                                             GroupId = x.GroupID,
                                             SchoolName = x.SchoolName,
                                             StudentId = x.StudentID,
                                             StudentName = x.StudentName,
                                             TeacherName = x.TeacherName,
                                             TeacherId = x.UserID,
                                             SchoolId = x.SchoolID,
                                             DistrictTermId = x.DistrictTermID,
                                             DistrictId = x.DistrictID
                                         });
        }
    }
}
