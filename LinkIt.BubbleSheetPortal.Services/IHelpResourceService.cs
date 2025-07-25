using LinkIt.BubbleSheetPortal.Models.HelpResource;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Services
{
    public interface IHelpResourceService
    {
        GetHelpResourcesResponse GetHelpResources(GetHelpResourcesRequest request);
        List<HelpResourceCategoryItem> GetHelpResourceCategories(int roleId);
        List<HelpResourceFileTypeItem> GetHelpResourceFileTypes();
        List<HelpResourceTypeItem> GetHelpResourceTypes();
        void SaveHelpResource(SaveHelpResourceRequest request);
        void DeleteHelpResource(int helpResourceID);
        GetHelpResourceByIDResponse GetHelpResourceByID(int helpResourceID);
    }
}
