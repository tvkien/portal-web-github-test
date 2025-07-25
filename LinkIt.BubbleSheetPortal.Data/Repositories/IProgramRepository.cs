using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IProgramRepository : IRepository<Program>
    {
        IQueryable<ProgramToView> GetProgramsByDistrictIdToView(int districtId, string currentDateTime);

        IQueryable<ListItem> GetProgramInStudentProgramByDistrictId(int districtId);
        List<ListItem> GetSurveyProgramByRole(int districtId, int userId, int roleId);
    }
}
