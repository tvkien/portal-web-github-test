using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models.HelpResource;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class HelpResourceService : IHelpResourceService
    {
        private IHelpResourceRepository _repository;

        public HelpResourceService(IHelpResourceRepository repository)
        {
            _repository = repository;
        }

        public GetHelpResourcesResponse GetHelpResources(GetHelpResourcesRequest request)
        {
            var result = _repository.GetHelpResources(request);

            return result;
        }

        public List<HelpResourceCategoryItem> GetHelpResourceCategories(int roleId)
        {
            var result = _repository.GetHelpResourceCategories(roleId);
            return result;
        }

        public List<HelpResourceFileTypeItem> GetHelpResourceFileTypes()
        {
            var result = _repository.GetHelpResourceFileTypes();
            return result;
        }

        public List<HelpResourceTypeItem> GetHelpResourceTypes()
        {
            var result = _repository.GetHelpResourceTypes();
            return result;
        }

        public void SaveHelpResource(SaveHelpResourceRequest request)
        {
            _repository.SaveHelpResource(request);
        }

        public void DeleteHelpResource(int helpResourceID)
        {
            _repository.DeleteHelpResource(helpResourceID);
        }

        public GetHelpResourceByIDResponse GetHelpResourceByID(int helpResourceID)
        {
            var result = _repository.GetHelpResourceByID(helpResourceID);
            return result;
        }
    }
}
