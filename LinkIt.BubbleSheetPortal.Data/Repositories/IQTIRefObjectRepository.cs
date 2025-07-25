using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.QTIRefObject;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IQTIRefObjectRepository
    {
        bool GetHasRightToEditPassage(int userId, int districtId, int qtiRefObjectId);
        IQueryable<QtiRefObject> GetQtiRefObject(GetQtiRefObjectFilter filter);
        IQueryable<QtiRefObject> GetAllQtiRefObjects();
        IEnumerable<QtiRefObjectVirtualTestDto> GetQtiRefObjectVirtualTests(int qtiRefObjectId);
    }
}
