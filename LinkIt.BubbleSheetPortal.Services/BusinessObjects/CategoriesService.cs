using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories.StudentPreferences;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentPreference;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services.BusinessObjects
{
    public class CategoriesService
    {
        private readonly StateService _stateService;
        private readonly DistrictService _districtService;
        private readonly SchoolService _schoolService;
        private readonly UserSchoolService _userSchoolService;
        private readonly IDataSetCategoryRepository _dataSetCategoryRepository;
        private readonly GenderService _genderService;
        private readonly IStudentPreferenceRepository _studentPreferenceRepository;

        public CategoriesService(StateService stateService,
            DistrictService districtService,
            SchoolService schoolService,
            UserSchoolService userSchoolService,
            GenderService genderService,
            IDataSetCategoryRepository dataSetCategoryRepository,
            IStudentPreferenceRepository studentPreferenceRepository           
            )
        {
            _stateService = stateService;
            _districtService = districtService;
            _schoolService = schoolService;
            _userSchoolService = userSchoolService;
            _genderService = genderService;
            _dataSetCategoryRepository = dataSetCategoryRepository;
            _studentPreferenceRepository = studentPreferenceRepository;          
        }

        public IQueryable<StateDTO> GetStates(int roleId, List<int> districtIds = null)
        {
            // get states for publisher
            IQueryable<State> data = _stateService.GetStates().OrderBy(x => x.Name);

            if (roleId != (int)Permissions.NetworkAdmin)
            {
                districtIds = new List<int>();
            }

            // filter states for network admin
            // return empty states for other roles
            if (roleId != (int)Permissions.Publisher)
            {
                var stateIds = _districtService.FilterDistricByIds(districtIds).Select(m => m.StateId).ToList();
                data = data.Where(m => stateIds.Contains(m.Id));
            }

            var list = data.Select(m => m.ConvertTo<StateDTO>());
            return list;
        }

        public IQueryable<SelectListItemDTO> GetDistrictByStateId(int roleId, int stateId, List<int> districtIds = null)
        {
            // get all district  for publisher
            if (roleId == (int)Permissions.Publisher)
            {
                districtIds = _districtService.GetDistrictsByStateId(stateId).Select(m => m.Id).ToList();
            }
            else if (roleId != (int)Permissions.NetworkAdmin) // return empty districts for other roles
            {
                districtIds = new List<int>();
            }

            var districts = _districtService.FilterDistricByIds(districtIds)
                .Where(m => m.StateId == stateId)
                .Select(m => new SelectListItemDTO
                {
                    Id = m.Id,
                    Name = m.Name
                }).OrderBy(m => m.Name);

            return districts;
        }

        public IQueryable<SelectListItemDTO> GetSchoolByDistrictId(int userId, int roleId, int? districtId)
        {
            IQueryable<SelectListItemDTO> data;

            if (districtId.HasValue)
            {
                //Publisher," + LabelHelper.DistrictLabel + " Admin, NetworkAdmin can see all schools
                if (roleId != (int)Permissions.SchoolAdmin)
                {
                    var schools = _schoolService.GetSchoolsByDistrictId(districtId.Value);
                    data = schools.Select(x => new SelectListItemDTO { Name = x.Name, Id = x.Id }).OrderBy(x => x.Name);
                }
                else
                {
                    //School Admin can see only schools that he/she has right access to
                    data = _userSchoolService.GetSchoolsUserHasAccessTo(userId).Select(
                                         x => new SelectListItemDTO { Id = x.SchoolId.GetValueOrDefault(), Name = x.SchoolName }).OrderBy(x => x.Name);
                }
            }
            else
            {
                data = GetSchoolsBasedOnPermissions(userId, roleId, districtId.GetValueOrDefault());
            }

            return data;
        }

        private IQueryable<SelectListItemDTO> GetSchoolsBasedOnPermissions(int userId, int roleId, int districtId)
        {
            if (roleId == (int)Permissions.DistrictAdmin || roleId == (int)Permissions.NetworkAdmin || roleId == (int)Permissions.Publisher)
            {
                return
                    _schoolService.GetSchoolsByDistrictId(districtId).Select(
                        x => new SelectListItemDTO { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);
            }

            return
                _userSchoolService.GetSchoolsUserHasAccessTo(userId).Select(
                    x => new SelectListItemDTO { Id = x.SchoolId.GetValueOrDefault(), Name = x.SchoolName }).OrderBy(x => x.Name);
        }

        public IEnumerable<Gender> GetAllGenders()
        {
            return _genderService.GetAllGenders();
        }

        public IQueryable<SelectListItemDTO> GetDataSetCategoriesAddListItem()
        {
            var result = _dataSetCategoryRepository.Select(c => new SelectListItemDTO()
            {
                Id = c.DataSetCategoryID,
                Name = c.DataSetCategoryName
            });
            return result;
        }

        public IList<Select2ListItem> GetDataSetCategoriesToComboTree(int? categoryId, int userId, int stateId, int districtId)
        {
            var dataSetCategories = _dataSetCategoryRepository.GetCategoryByUser(userId, stateId, districtId, categoryId).OrderBy(x => x.DataSetCategoryName).ToList();

            return ConvertToSelectTwo(dataSetCategories);
        }

        public IList<Select2ListItem> ConvertToSelectTwo(List<DataSetCategoryDTO> dataSetCategories)
        {
            return dataSetCategories
                 .OrderBy(x => x.CombinedDisplayText)
                 .Select(o => new Select2ListItem
                 {
                     Text = o.CombinedDisplayText,
                     Id = o.DataSetCategoryID.ToString()
                 })
                 .ToList();
        }

        public IEnumerable<Select2ListItem> GetDataSetCategories(GetDatasetCatogoriesParams catogoriesParams)
        {
            var dataSetCategories = _studentPreferenceRepository.GetDataSetCategories(catogoriesParams).ToList();
            return ConvertToSelectTwo(dataSetCategories);
        }

        public IQueryable<DataSetCategory> DataSetCategories => _dataSetCategoryRepository.Select();

        public IQueryable<DataSetCategory> GetCategoriesByListCategoryID(List<int> categoryIds)
        {
            return _dataSetCategoryRepository.Select().Where(c => categoryIds.Contains(c.DataSetCategoryID));
        }
    }
}
