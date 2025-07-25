using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IUserStudentRepository : IReadOnlyRepository<UserStudent>
    {
        List<GetAvailableClassesBySchoolAndStudentIdResult> GetAvailableClassesBySchoolAndStudentId(int schoolId, int studentId, int? userId, int offSetRowCount, int fetchRowCount, string searchStr, string sortBy, string sortDirection);
    }
}

