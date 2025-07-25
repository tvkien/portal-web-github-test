using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetDataRepository : IReadOnlyRepository<BubbleSheetData>
    {
        private readonly Table<BubbleSheetDataView> table;

        public BubbleSheetDataRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = BubbleSheetDataContext.Get(connectionString).GetTable<BubbleSheetDataView>();
        }

        public IQueryable<BubbleSheetData> Select()
        {
            return table.Select(x => new BubbleSheetData
                                         {
                                            TestId = x.VirtualTestID,
                                            BankId = x.BankID,
                                            StateId = x.StateID,
                                            GradeId = x.GradeID,
                                            SubjectId = x.SubjectID,
                                            DistrictId = x.DistrictID,
                                            ClassId = x.ClassID,
                                            SchoolId = x.SchoolID,
                                            ClassName = x.ClassName,
                                            SchoolName = x.SchoolName,
                                            SubjectName = x.SubjectName,
                                            TestName = x.TestName,
                                            DistrictName = x.DistrictName,
                                            TeacherName = x.NameLast,
                                            UserId = x.UserID,
                                            DistrictTermId = x.DistrictTermID
                                         });
        }
    }
}