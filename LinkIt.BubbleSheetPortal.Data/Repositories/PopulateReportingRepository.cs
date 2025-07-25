using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class PopulateReportingRepository : IPopulateReportingRepository
    {
        private readonly TestDataContext dbContext;

        public PopulateReportingRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = TestDataContext.Get(connectionString);
        }

        public List<Grade> ReportingGetGrades(int userId, int districtId, int roleId, int virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            var query = dbContext.ReportingGetGrade(districtId, userId, roleId, virtualTestSubTypeId, resultDateFrom, resultDateTo, (isGetAllClass.HasValue && isGetAllClass.Value) ? 1 : 0);
            if (query != null)
            {
                return query.Select(g => new Grade()
                {
                    Id = g.GradeID,
                    Name = g.Name,
                    Order = g.Order
                }).OrderBy(g => g.Order).ToList();
            }
            return new List<Grade>();
        }

        public List<Subject> ReportingGetSubjects(int gradeId, int districtId, int userId, int userRole, int virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            var tmp = dbContext.ReportingGetSubject(gradeId, districtId, userId, userRole,virtualTestSubTypeId, resultDateFrom, resultDateTo, (isGetAllClass.HasValue && isGetAllClass.Value) ? 1 : 0);
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

        public List<ListItem> ReportingGetBanks(int subjectId, int districtId, int userId, int userRole, int virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            var tmp = dbContext.ReportingGetBank(subjectId, districtId, userId, userRole, virtualTestSubTypeId, resultDateFrom, resultDateTo, (isGetAllClass.HasValue && isGetAllClass.Value) ? 1 : 0);
            if (tmp != null)
                return tmp.Select(o => new ListItem
                {
                    Id = o.bankId,
                    Name = o.name
                }).ToList();
            return new List<ListItem>();
        }

        public List<ListItem> ReportingGetTests(int? gradeId, int? subjectId, int? bankId, int districtId, int userId, int userRole, int virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            var tmp = dbContext.ReportingGetVirtualTest(gradeId, subjectId, bankId, districtId, userId, userRole, virtualTestSubTypeId, resultDateFrom, resultDateTo, (isGetAllClass.HasValue && isGetAllClass.Value) ? 1 : 0);
            if (tmp != null)
                return tmp.Select(o => new ListItem
                {
                    Id = o.VirtualTestID,
                    Name = o.Name
                }).ToList();
            return new List<ListItem>();
        }

    }
}
