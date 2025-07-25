using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IStudentTestDistrictTermRepository : IReadOnlyRepository<StudentTestDistrictTerm>
    {
        IEnumerable<StudentTestDistrictTerm> GetStudentTestDistrictTerms(StudentTestDistrictTermParam param);
    }
}
