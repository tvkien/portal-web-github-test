using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ISchoolTeacherRepository : IReadOnlyRepository<SchoolTeacher>
    {
        List<ListItem> GetListTeacherBySchoolIdAndDistrictTermId(int schoolId, int districttermId, int userId = 0, int roleId = 0);
        List<ListItem> GetAllListTeacherBySchoolIdAndDistrictTermId(int schoolId, int districttermId, int userId = 0, int roleId = 0);
    }
}
