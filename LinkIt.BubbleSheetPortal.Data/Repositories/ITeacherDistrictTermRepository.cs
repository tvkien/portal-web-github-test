using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ITeacherDistrictTermRepository : IReadOnlyRepository<TeacherDistrictTerm>
    {
        IEnumerable<TeacherDistrictTerm> GetTermBySchool(int schoolId, int userId, int roleId);
    }
}
