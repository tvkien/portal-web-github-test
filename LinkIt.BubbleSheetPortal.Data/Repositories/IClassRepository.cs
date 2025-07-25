using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.AggregateSubjectMapping;
using LinkIt.BubbleSheetPortal.Models.DTOs.Classes;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IClassRepository
    {
        bool SaveClassMeta(List<CreateClassMetas> model);
        List<ClassMetaDto> GetMetaClassByClassId(List<int> classId);
        IQueryable<Class> SelectWithoutFilterByActiveTerm();
        Class GetClassByID(int classID);
        Class GetClassBySchoolTermAndUser(int districtId, int schoolId, int districtTermId, string strClassName);
        List<ListItem> GetClassBySchoolAndTerm(int districtId, int schoolId, int districtTermId, int userId, int roleId);
        List<ListItem> GetClassBySchoolAndTermV2(int districtId, string schoolIds, int districtTermId, int userId, int roleId);
        IEnumerable<ListItem> GetClassDistrictTermBySchool(int schoolId, int? userId, int? roleId);
    }
}
