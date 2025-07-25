using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentPreference;
using LinkIt.BubbleSheetPortal.Models.RestrictionDTO;
using LinkIt.BubbleSheetPortal.Services.BusinessObjects;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TestRestrictionModuleService
    {
        private readonly ITestRestrictionModuleRepository testRestrictionModuleRepository;
        private readonly IVirtualTestRepository virtualTestRepository;       
        private readonly CategoriesService _categoriesService;

        public TestRestrictionModuleService(ITestRestrictionModuleRepository testRestrictionModuleRepository,
            IVirtualTestRepository virtualTestRepository, CategoriesService categoriesService)
        {
            this.testRestrictionModuleRepository = testRestrictionModuleRepository;
            this.virtualTestRepository = virtualTestRepository;
            this._categoriesService = categoriesService;            
        }
        public TestRestrictionModuleService(ITestRestrictionModuleRepository testRestrictionModuleRepository
           )
        {
            this.testRestrictionModuleRepository = testRestrictionModuleRepository;
        }
        public List<TestRestrictionModuleDTO>
            GetTestRestrictionModuleRoleByBankAndDistrict
            (GetTestRestrictionModuleRoleByBankAndDistrictReuqestDTO getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO)
        {
            return testRestrictionModuleRepository.GetTestRestrictionModuleRoleByBankAndDistrict
                (getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO);

        }
        public List<TestRestrictionModuleDTO>
          GetTestRestrictionModuleRoleByBankTestAndDistrict
          (GetTestRestrictionModuleRoleByBankAndDistrictReuqestDTO getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO)
        {
            return testRestrictionModuleRepository.GetTestRestrictionModuleRoleByBankTestAndDistrict
                (getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO);

        }
        public void SaveTestRestrictionModule(SaveTestRestrictionModuleRequestDTO saveTestRestrictionModuleRequestDTO)
        {
            testRestrictionModuleRepository.SaveTestRestrictionModule(saveTestRestrictionModuleRequestDTO);
        }
        public void DeleteAllRetrictTestBanFromBankIdAndDistrict(DeleteAllRetrictBankTestFromBankIdAndDistrictRequestDTO
          deleteAllRetrictBankFromBankIdAndDistrictRequestDTO)
        {
            //get Virtualtestid from bankid
            deleteAllRetrictBankFromBankIdAndDistrictRequestDTO.ListVirtualTest = virtualTestRepository.Select().
                Where(x => x.BankID == deleteAllRetrictBankFromBankIdAndDistrictRequestDTO.BankID).Select(x => x.VirtualTestID).ToList();

            testRestrictionModuleRepository.DeleteAllRetrictBankTestFromBankIdAndDistrict
                (deleteAllRetrictBankFromBankIdAndDistrictRequestDTO);
        }
        public List<RestrictionDTO> GetListRestriction(string moduleCode, int userId, int roleId, PublishLevelTypeEnum publishLevelType, int publishLevelId)
        {
            return testRestrictionModuleRepository.GetListRestriction(moduleCode, userId, roleId, publishLevelType, publishLevelId);
        }
        public List<RestrictionDTO> GetListRestriction(int userId, int roleId, int districtId)
        {
            return testRestrictionModuleRepository.GetListRestriction(userId, roleId, districtId);
        }
        public void SaveRestrictionCategoriesTests(SaveCategoriesTestsRestrictionModuleRequestDto saveCategoryTestRestrictionModuleRequestDTO)
        {
            testRestrictionModuleRepository.SaveRestrictionCategoryTest(saveCategoryTestRestrictionModuleRequestDTO);
        }
        public List<CategoryRestrictionModuleDto> GetCategoryRestrictions(GetDatasetCatogoriesParams criteria)
        {
            var categories = _categoriesService.GetDataSetCategories(criteria);
            var categoriesResult = categories.Select(c => new SelectListItemDTO { Id = int.TryParse(c.Id,out int result) ? result : 0 , Name = c.Text }).ToList();
            return testRestrictionModuleRepository.GetCategoriesRestriction(criteria.DistrictId, categoriesResult);
        }
        public List<CategoryRestrictionModuleDto> GetCategoriesRestriction(int districtId, List<SelectListItemDTO> categories)
        {
            return testRestrictionModuleRepository.GetCategoriesRestriction(districtId, categories);
        }
        public TestForRetrictionResponseDto GetTestRestrictions(TestRestrictionRequestDto criteria)
        {
            return testRestrictionModuleRepository.GetTestsRestriction(criteria);
        }
        public CategoryTestRestrictResponseDto GetRestrictionByRestrictedObject(int categoryId, int virtualTestId, int districtId)
        {
            return testRestrictionModuleRepository.GetRestrictionByRestrictedObject(categoryId, virtualTestId, districtId);
        }


    }
}
