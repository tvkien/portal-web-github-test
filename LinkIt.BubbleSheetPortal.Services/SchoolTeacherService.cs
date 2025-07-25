using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SchoolTeacherService
    {
        private readonly ISchoolTeacherRepository repository;

        public SchoolTeacherService(ISchoolTeacherRepository repository)
        {
            this.repository = repository;
        }

        public IQueryable<SchoolTeacher> GetTeachersBySchoolId(int schoolId)
        {
            return repository.Select().Where(x => x.SchoolId.Equals(schoolId));
        }

        public List<ListItem> GetListTeacherBySchoolIdAndDistrictTermId(int schoolid, int districttermId, int userId = 0, int roleId = 0)
        {
            return repository.GetListTeacherBySchoolIdAndDistrictTermId(schoolid, districttermId, userId, roleId);
        }
        public List<ListItem> GetAllListTeacherBySchoolIdAndDistrictTermId(int schoolid, int districttermId, int userId = 0, int roleId = 0)
        {
            return repository.GetAllListTeacherBySchoolIdAndDistrictTermId(schoolid, districttermId, userId, roleId);
        }
    }
}
