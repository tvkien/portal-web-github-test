using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IListRubricRepository : IReadOnlyRepository<ListRubric>
    {
        IEnumerable<ListRubric> GetRubrics(int districtID, int userID, int roleID,
            int? gradeID, int? subjectID, string bankName, string authorName, string virtualTestName, int pageIndex, int pageSize, ref int? totalRecords, string sortColumns);

        IEnumerable<ListRubric> GetListRubrics(RubricCustomList param, ref int? totalRecords);
    }

}
