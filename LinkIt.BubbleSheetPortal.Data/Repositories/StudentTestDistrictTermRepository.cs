using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StudentTestDistrictTermRepository : IStudentTestDistrictTermRepository
    {
        private readonly Table<StudentTestDistrictTermView> table;
        private readonly UserDataContext dbContext;

        public StudentTestDistrictTermRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<StudentTestDistrictTermView>();
            dbContext = UserDataContext.Get(connectionString);
        }

        public IEnumerable<StudentTestDistrictTerm> GetStudentTestDistrictTerms(StudentTestDistrictTermParam param)
        {
            var virtualTestIdsString = param.VirtualTestIds.ConvertToString(";");
            var virtualTestSubTypeIdsString = param.VirtualTestSubTypeIds.ConvertToString(";");

            return dbContext.GetStudentTestDistrictTerm(
                    param.DistrictId,
                    param.SchoolId,
                    param.TeacherId,
                    param.DistrictTermId,
                    param.ClassId,
                    virtualTestIdsString,
                    virtualTestSubTypeIdsString,
                    param.ResultDateFrom,
                    param.ResultDateTo)
                .Select(x => new StudentTestDistrictTerm
                {
                    UserId = x.UserId.Value,
                    UserName = x.UserName,
                    NameFirst = x.NameFirst,
                    NameLast = x.NameLast,
                    DistrictTermId = x.DistrictTermID.Value,
                    DistrictTermName = x.DistrictTermName,
                    DateStart = x.DateStart,
                    ClassId = x.ClassId.Value,
                    ClassName = x.ClassName,
                    SchoolId = x.SchoolID,
                    SchoolName = x.SchoolName,
                    StudentId = x.StudentID.Value,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                });
        }

        public IQueryable<StudentTestDistrictTerm> Select()
        {
            return table.Select(x => new StudentTestDistrictTerm
            {
                DateEnd = x.DateEnd,
                DateStart = x.DateStart,
                DistrictTermId = x.DistrictTermID,
                DistrictTermName = x.DistrictTermName,
                UserId = x.UserID,
                UserName = x.UserName,
                NameFirst = x.NameFirst,
                NameLast = x.NameLast,
                VirtualTestId = x.VirtualTestID,
                ClassId = x.ClassID,
                ClassName = x.ClassName,
                SchoolId = x.SchoolID,
                DistrictId = x.DistrictID,
                SchoolName = x.SchoolName,
                VirtualTestSubTypeId = x.VirtualTestSubTypeID,
                ResultDate = x.ResultDate,
                FirstName = x.FirstName,
                LastName = x.LastName,
                StudentId = x.StudentID,
                TakenTestClassId = x.TakenTestClassID,
                TakenTestDistrictTermId = x.TakenTestDistrictTermID.GetValueOrDefault(),
                TakenTestSchoolId = x.TakenTestSchoolID.GetValueOrDefault(),
                TakenTestTeacherId = x.TakenTestTeacherID
            });
        }
    }
}
