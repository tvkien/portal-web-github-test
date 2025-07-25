using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ExtractTestResultService
    {
        private readonly IReadOnlyRepository<ExtractTestResult> repository;

        public ExtractTestResultService(IReadOnlyRepository<ExtractTestResult> repository)
        {
            this.repository = repository;
        }
         
        public IQueryable<ExtractTestResult> GetListExtractTestResultByItems( ExtractTestResultCustom obj)
        {
            if (obj == null) return repository.Select().Where(o => o.DistrictId == -1);

            var query =  repository.Select().Where(o => o.DistrictId == obj.districtId.Value && o.StudentDistrictId == obj.districtId);

            if (!string.IsNullOrEmpty(obj.GradeName))
            {
                query = query.Where(o => o.GradeName.Contains(obj.GradeName.Trim()));
            }
            if (!string.IsNullOrEmpty(obj.SubjectName))
            {
                query = query.Where(o => o.SubjectName.Contains(obj.SubjectName.Trim()));
            }
            if (!string.IsNullOrEmpty(obj.BankName))
            {
                query = query.Where(o => o.BankName.Contains(obj.BankName.Trim()));
            }
            if (!string.IsNullOrEmpty(obj.TestName))
            {
                query = query.Where(o => o.TestName.Contains(obj.TestName.Trim()));
            }
            if (!string.IsNullOrEmpty(obj.SchoolName))
            {
                query = query.Where(o => o.SchoolName.Contains(obj.SchoolName.Trim()));
            }
            if (!string.IsNullOrEmpty(obj.TeacherName))
            {
                query = query.Where(o => o.TeacherCustom.Contains(obj.TeacherName.Trim()));
            }
            if (!string.IsNullOrEmpty(obj.ClassName))
            {
                query = query.Where(o => o.ClassName.Contains(obj.ClassName.Trim()));
            }
            if (!string.IsNullOrEmpty(obj.StudentName))
            {
                query = query.Where(o => o.StudentCustom.Contains(obj.StudentName.Trim()));
            }

            DateTime dtFromDate = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(obj.FromDate) && DateTime.TryParse(obj.FromDate.Trim(),out dtFromDate))
            {
                query = query.Where(o => o.ResultDate >= dtFromDate);
            }

            DateTime dtToDate = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(obj.FromDate) && DateTime.TryParse(obj.ToDate.Trim(), out dtToDate))
            {
                query = query.Where(o => o.ResultDate <= dtToDate);
            }
            if (obj.RoleId == (int) Permissions.Publisher || obj.RoleId == (int) Permissions.DistrictAdmin)
            {
                query = query.Where(o => o.StudentDistrictId == obj.districtId);
            }
            return query;
        }
    }
}
