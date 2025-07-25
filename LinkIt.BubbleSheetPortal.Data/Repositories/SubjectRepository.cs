using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserDefinedTypesDto;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly Table<SubjectEntity> table;
        private readonly Table<GradeEntity> gradeTable;
        private readonly Table<SubjectOrderEntity> subjectOrderTable;
        private readonly TestDataContext _testDataContext;

        public SubjectRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _testDataContext = TestDataContext.Get(connectionString);
            table = _testDataContext.GetTable<SubjectEntity>();
            gradeTable = _testDataContext.GetTable<GradeEntity>();
            subjectOrderTable = _testDataContext.GetTable<SubjectOrderEntity>();
        }

        public IQueryable<Subject> Select()
        {
            return table.Select(x => new Subject
            {
                Id = x.SubjectID,
                Name = x.Name,
                GradeId = x.GradeID,
                StateId = x.StateID,
                ShortName = x.ShortName
            });
        }

        public List<Subject> GetSubjectSByGradeId(int gradeId, int districtId, int userId, int userRole)
        {
            var tmp = _testDataContext.ProcSubject(gradeId, districtId, userId, userRole);
            if (tmp != null)
                return tmp.Select(o => new Subject
                {
                    Id = o.SubjectID,
                    Name = o.Name,
                    GradeId = o.GradeID,
                    StateId = o.StateID,
                    ShortName = o.ShortName
                }).ToList();
            return new List<Subject>();
        }

        public List<Subject> ACTGetSubjectSByGradeId(int gradeId, int districtId, int userId, int userRole)
        {
            var tmp = _testDataContext.ACTProcSubject(gradeId, districtId, userId, userRole);
            if (tmp != null)
                return tmp.Select(o => new Subject
                {
                    Id = o.SubjectID,
                    Name = o.Name,
                    GradeId = o.GradeID,
                    StateId = o.StateID,
                    ShortName = o.ShortName
                }).ToList();
            return new List<Subject>();
        }
        public List<Subject> GetSubjectsForItemSetSaveTest(int gradeId, int districtId, int userId, int userRole)
        {
            var tmp = _testDataContext.GetSubjectsForItemSetSaveTest(gradeId, districtId, userId, userRole);
            if (tmp != null)
                return tmp.Select(o => new Subject
                {
                    Id = o.SubjectID,
                    Name = o.Name,
                    GradeId = o.GradeID,
                    StateId = o.StateID,
                    ShortName = o.ShortName
                }).ToList();
            return new List<Subject>();
        }
        public List<Subject> GetSubjectsByGradeIdAndAuthorOfAllTestType(SearchBankCriteria criteria)
        {

            var subjects = _testDataContext.GetSubjectsByGradeIdAndAuthorOfAllTestType(criteria.GradeId, criteria.DistrictId, criteria.UserId, criteria.UserRole);
            if (subjects != null)
                return subjects.Select(o => new Subject
                {
                    Id = o.SubjectID,
                    Name = o.Name,
                    GradeId = o.GradeID,
                    StateId = o.StateID,
                    ShortName = o.ShortName
                }).ToList();
            return new List<Subject>();
        }
        public List<Subject> GetSubjectSByGradeIdAndAuthor(SearchBankCriteria criteria)
        {
            var tmp = _testDataContext.GetSubjectByGradeIdAndAuthor(criteria.GradeId, criteria.DistrictId, criteria.UserId, criteria.UserRole);
            if (tmp != null)
                return tmp.Select(o => new Subject
                {
                    Id = o.SubjectID,
                    Name = o.Name,
                    GradeId = o.GradeID,
                    StateId = o.StateID,
                    ShortName = o.ShortName
                }).ToList();
            return new List<Subject>();
        }

        public List<Subject> SATGetSubjectSByGradeId(int gradeId, int districtId, int userId, int userRole)
        {
            var tmp = _testDataContext.SATProcSubject(gradeId, districtId, userId, userRole);
            if (tmp != null)
                return tmp.Select(o => new Subject
                {
                    Id = o.SubjectID,
                    Name = o.Name,
                    GradeId = o.GradeID,
                    StateId = o.StateID,
                    ShortName = o.ShortName
                }).ToList();
            return new List<Subject>();
        }

        public List<SubjectOrder> GetSubjectsFormBankByGradeId(int gradeId, int districtId, int userId, int userRole, bool isFromMultiDate, bool usingMultiDate)
        {
            var tmp = _testDataContext.ProcSubjectFormBank(gradeId, districtId, userId, userRole, isFromMultiDate, usingMultiDate);
            if (tmp != null)
                return tmp.Select(o => new SubjectOrder
                {
                    Id = o.SubjectID,
                    Name = o.Name,
                    GradeId = o.GradeID,
                    StateId = o.StateID,
                    ShortName = o.ShortName,
                    Order = o.SortOrder.Value
                }).OrderBy(o => o.Order).ThenBy(o => o.Name).ToList();
            return new List<SubjectOrder>();
        }

        public IQueryable<SubjectOrderDistrict> GetSubjectOrders(int districtId)
        {
            return subjectOrderTable.Where(x => x.DistrictID == districtId).Select(x => new SubjectOrderDistrict() { SubjectId = x.SubjectID, Order = x.Order });
        }

        public Subject GetSubjectByShortName(string shortName, int stateId, string subjectName, string gradeName)
        {
            return _testDataContext.GetSubjectByShortName(stateId, shortName, subjectName, gradeName)
                .Select(o => new Subject
                {
                    Id = o.SubjectID.GetValueOrDefault(),
                    Name = o.Name,
                    GradeId = o.GradeId.GetValueOrDefault(),
                    StateId = stateId,
                    ShortName = o.ShortName
                }).FirstOrDefault();
        }

    }
}
