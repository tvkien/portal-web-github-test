using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ExtractTestResultRepository : IReadOnlyRepository<ExtractTestResult>
    {
        private readonly Table<ExtractTestResultsView> table;

        public ExtractTestResultRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = ExtractTestDataContext.Get(connectionString).GetTable<ExtractTestResultsView>();
        }

        public IQueryable<ExtractTestResult> Select()
        {
            return table.Select(x => new ExtractTestResult()
                                {
                                   TestResultId = x.TestResultID,
                                   TestNameCustom = x.TestNameCustom,
                                   ClassNameCustom = x.ClassNameCustom,
                                   StudentCustom = x.StudentCustom,
                                   StudentId = x.StudentID,
                                   TeacherCustom = x.TeacherCustom,
                                   SchoolName = x.SchoolName,
                                   DistrictId = x.DistrictID,
                                   TestName = x.TestName,
                                   ResultDate = x.UpdatedDate,
                                   GradeName = x.GradeName,
                                   SubjectName = x.SubjectName,
                                   BankName = x.BankName,
                                   ClassName = x.ClassName,
                                   StudentDistrictId = x.StudentDistrictID,
                                   SchoolId = x.SchoolID ?? 0,
                                   UserId = x.UserID ?? 0,
                                   ClassId = x.ClassID ?? 0
                                });
        }
    }
}
