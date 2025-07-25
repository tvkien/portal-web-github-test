using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.HelpResource;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class HelpResourceRepository : IHelpResourceRepository
    {
        private readonly TestDataContext _context;

        public HelpResourceRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = TestDataContext.Get(connectionString);
        }

        public GetHelpResourcesResponse GetHelpResources(GetHelpResourcesRequest request)
        {
            int? totalRecord = 0;
            var result = new GetHelpResourcesResponse();
            var data = _context.GetHelpResources(request.SearchText, request.HelpResourceCaterogyIDs, request.RoleId, request.SortColumn, request.SortDirection, request.StartRow, request.PageSize, ref totalRecord).ToList();
            result.Data = data.Select(o => Transform(o)).ToList();
            result.TotalRecord = totalRecord.HasValue ? totalRecord.Value : 0;

            return result;
        }

        public List<HelpResourceCategoryItem> GetHelpResourceCategories(int roleId)
        {
            var data = _context.GetHelpResourceCategoriesByRole(roleId).ToList();
            var result = data.Select(o => Transform(o)).ToList();

            return result;
        }

        public List<HelpResourceFileTypeItem> GetHelpResourceFileTypes()
        {
            var data = _context.GetHelpResourceFileTypes().ToList();
            var result = data.Select(o => Transform(o)).ToList();

            return result;
        }

        public List<HelpResourceTypeItem> GetHelpResourceTypes()
        {
            var data = _context.GetHelpResourceTypes().ToList();
            var result = data.Select(o => Transform(o)).ToList();

            return result;
        }

        public void SaveHelpResource(SaveHelpResourceRequest request)
        {
            _context.SaveHelpResource(request.HelpResourceID,
                request.HelpResourceTypeID,
                request.HelpResourceCategoryID,
                request.HelpResourceFileTypeID,
                request.Topic,
                request.Description,
                request.KeyWords,
                request.HelpResourceFilePath,
                request.HelpResourceLink);
        }

        public void DeleteHelpResource(int helpResourceID)
        {
            _context.DeleteHelpResource(helpResourceID);
        }

        public GetHelpResourceByIDResponse GetHelpResourceByID(int helpResourceID)
        {
            var data = _context.GetHelpResourceByID(helpResourceID).FirstOrDefault();
            return Transform(data);
        }

        private GetHelpResourceByIDResponse Transform(GetHelpResourceByIDResult data)
        {
            if (data == null) return null;

            var result = new GetHelpResourceByIDResponse
            {
                DateUpdated = data.DateUpdated,
                Description = data.Description,
                HelpResourceFilePath = data.HelpResourceFilePath,
                HelpResourceFileTypeID = data.HelpResourceFileTypeID,
                HelpResourceID = data.HelpResourceID,
                HelpResourceLink = data.HelpResourceLink,
                HelpResourceTypeID = data.HelpResourceTypeID,
                Topic = data.Topic,
                DateCreated = data.DateCreated,
                HelpResourceCategoryID = data.HelpResourceCategoryID,
                KeyWords = data.KeyWords
            };

            return result;
        }

        private HelpResourcesSearchItem Transform(GetHelpResourcesResult data)
        {
            if (data == null) return null;

            var result = new HelpResourcesSearchItem
            {
                CategoryText = data.CategoryText,
                DateUpdated = data.DateUpdated,
                Description = data.Description,
                HelpResourceFilePath = data.HelpResourceFilePath,
                HelpResourceFileTypeID = data.HelpResourceFileTypeID,
                HelpResourceID = data.HelpResourceID,
                HelpResourceLink = data.HelpResourceLink,
                HelpResourceTypeIcon = data.HelpResourceTypeIcon,
                HelpResourceTypeID = data.HelpResourceTypeID,
                Topic = data.Topic,
                HelpResourceFileTypeName = data.HelpResourceFileTypeName,
                HelpResourceTypeDisplayText = data.HelpResourceTypeDisplayText
            };

            return result;
        }

        private HelpResourceCategoryItem Transform(GetHelpResourceCategoriesByRoleResult data)
        {
            if (data == null) return null;

            var result = new HelpResourceCategoryItem
            {
               DisplayText = data.DisplayText,
               HelpResourceCategoryID = data.HelpResourceCategoryID,
               SortOrder = data.SortOrder.HasValue ? data.SortOrder.Value : 0,
               Code = data.Code
            };

            return result;
        }

        private HelpResourceFileTypeItem Transform(GetHelpResourceFileTypesResult data)
        {
            if (data == null) return null;

            var result = new HelpResourceFileTypeItem
            {
                DisplayText = data.Name,
                HelpResourceFileTypeID = data.HelpResourceFileTypeID,
                Extensions = data.Extensions
            };

            return result;
        }

        private HelpResourceTypeItem Transform(GetHelpResourceTypesResult data)
        {
            if (data == null) return null;

            var result = new HelpResourceTypeItem
            {
                DisplayText = data.DisplayText,
                HelpResourceTypeID = data.HelpResourceTypeID,
                Description = data.Description,
                ImgPath = data.ImagePath
            };

            return result;
        }
    }
}
