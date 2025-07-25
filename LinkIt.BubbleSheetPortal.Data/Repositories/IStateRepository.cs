using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IStateRepository : IReadOnlyRepository<State>
    {
        List<State> GetStateForUser(int userId, int districtId, bool useStateOfDistrictSchool = false, bool useStateSchool = false);
        string GetTimeZoneId(int stateId);
    }
}
