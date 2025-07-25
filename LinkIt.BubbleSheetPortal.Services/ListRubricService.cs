using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ListRubricService
    {
        private readonly IListRubricRepository repository;

        public ListRubricService(IListRubricRepository repository)
        {
            this.repository = repository;
        }

        public IQueryable<ListRubric> GetListRubrics (int userId)
        {
            return repository.Select().Where(o => o.AuthorUserId == userId);
        }

        public IQueryable<ListRubric> GetListRubrics(IQueryable<ListRubric> lstQuery, int gradeId, int subjectId, string bankName, string author, string testName)
        {
            var query = lstQuery;
            if (gradeId != 0)
            {
                query = query.Where(o => o.GradeId == gradeId);
            }
            if (subjectId != 0)
            {
                query = query.Where(o => o.SubjectId == subjectId);
            }            
            if (!string.IsNullOrEmpty(bankName))
            {
                query = query.Where(o => o.BankName.Contains(bankName.Trim()));
            }
            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(o => o.Author.Contains(author.Trim()));
            }
            if (!string.IsNullOrEmpty(testName))
            {
                query = query.Where(o => o.TestName.Contains(testName.Trim()));
            }
            return query;
        }

        public IQueryable<ListRubric> GetListRubricsByRole(int userId, int roleId, int districtId, List<int>lstTeacherId )
        {
            var query = repository.Select();
            switch (roleId)
            {
                case (int)Permissions.Publisher:
                case (int)Permissions.NetworkAdmin:
                case (int)Permissions.DistrictAdmin:
                    {
                        query = query.Where(o => o.DistrictId == districtId || o.BankShareDistrictID == districtId);
                    } break;
                case (int)Permissions.SchoolAdmin:
                    {
                        if (lstTeacherId != null && lstTeacherId.Count > 0)
                        {
                            query = query.Where(o => lstTeacherId.Contains(o.AuthorUserId));
                        }
                    } break;
                default:
                    {
                        query = query.Where(o => o.AuthorUserId == userId);
                    }
                    break;
            }
            return query;
        }

        public IEnumerable<ListRubric> GetRubrics(int districtID, int userID, int roleID,
            int? gradeID, int? subjectID, string bankName, string authorName, string virtualTestName, int pageIndex, int pageSize, ref int? totalRecords, string sortColumns)
        {
            var result = repository.GetRubrics(districtID, userID, roleID, gradeID, subjectID, bankName,
                authorName, virtualTestName, pageIndex, pageSize, ref totalRecords, sortColumns);

            return result;
        }
        public IEnumerable<ListRubric> GetListRubricsBySubjectName(RubricCustomList param, ref int? totalRecords)
        {
            return repository.GetListRubrics(param, ref totalRecords);
        }
    }
}
