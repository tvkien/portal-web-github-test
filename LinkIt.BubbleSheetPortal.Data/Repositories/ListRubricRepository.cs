using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ListRubricRepository : IListRubricRepository
    {
         private readonly Table<ListRubricView> table;
        private readonly TestDataContext _testDataContext;

        public ListRubricRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<ListRubricView>();
            _testDataContext = TestDataContext.Get(connectionString);
        }

        public IQueryable<ListRubric> Select()
        {
            return table.Select(o => new ListRubric()
            {
                SubjectName = o.SubjectName,
                GradeName = o.GradeName,
                BankName = o.BankName,
                Author = o.Author,
                TestName = o.TestName,
                FileName = o.FileName,
                DistrictId = o.DistrictID.HasValue ? o.DistrictID.Value : 0,
                GradeId = o.GradeID,
                SubjectId = o.SubjectID,
                VirtualTestFileId = o.VirtualTestFileID.HasValue ? o.VirtualTestFileID.Value : 0,
                TestId = o.VirtualTestID,
                FileKey = o.FileKey,
                BankShareDistrictID = o.BankShareDistrictID.HasValue ? o.BankShareDistrictID.Value : 0,
                AuthorUserId = o.AuthorUserID.HasValue ? o.AuthorUserID.Value : 0,
                BankId = o.BankID
            });
        }
        public IEnumerable<ListRubric> GetRubrics(int districtID, int userID, int roleID,
            int? gradeID, int? subjectID, string bankName, string authorName, string virtualTestName, int pageIndex, int pageSize, ref int? totalRecords, string sortColumns)
        {
            //Temporary set SubjectName as empty
            var rubrics = _testDataContext.GetRubrics(districtID, userID, roleID, gradeID, subjectID,"", bankName,
                authorName, virtualTestName, pageIndex, pageSize, ref totalRecords, sortColumns).ToList();
            var result = rubrics.Select(x => new ListRubric
            {
                SubjectName = x.SubjectName,
                SubjectId = x.SubjectID,
                GradeId = x.GradeID,
                GradeName = x.GradeName,
                BankName = x.BankName,
                Author = x.Author,
                BankId = x.BankID,
                FileName = x.FileName,
                VirtualTestFileId = x.VirtualTestFileID ?? 0,
                FileKey = x.FileKey,
                TestName = x.TestName,
                AuthorUserId = x.AuthorUserID ?? 0,
                DistrictId = x.DistrictID ?? 0,
                TestId = x.VirtualTestID
            });

            return result;
        }
        public IEnumerable<ListRubric> GetListRubrics(RubricCustomList param, ref int? totalRecords)
        {
            var tmp = _testDataContext.GetRubrics(param.DistrictId, param.UserId, param.UserRole, param.GradeId, param.SubjectId, param.SubjectName, param.BankName, param.Author, param.TestName, param.PageIndex, param.PageSize, ref totalRecords, param.SortColumn);
            if (tmp != null)
                return tmp.Select(o => new ListRubric()
                {
                    SubjectName = o.SubjectName,
                    GradeName = o.GradeName,
                    BankName = o.BankName,
                    Author = o.Author,
                    TestName = o.TestName,
                    FileName = o.FileName,
                    DistrictId = o.DistrictID.HasValue ? o.DistrictID.Value : 0,
                    GradeId = o.GradeID,
                    SubjectId = o.SubjectID,
                    VirtualTestFileId = o.VirtualTestFileID.HasValue ? o.VirtualTestFileID.Value : 0,
                    TestId = o.VirtualTestID,
                    FileKey = o.FileKey
                });

            return null;
        }

    }
}
