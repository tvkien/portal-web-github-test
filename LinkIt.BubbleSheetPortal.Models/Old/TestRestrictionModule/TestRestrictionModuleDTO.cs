using LinkIt.BubbleSheetPortal.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestRestrictionModuleRoleDTO
    {
        public TestRestrictionModuleRoleDTO()
        {
            IsChecked = false;
            IsShown = true;
        }
        public int RoleID { set; get; }

        public string RoleName { set; get; }
        public string RoleDisplayName { set; get; }

        public bool IsDisable { set; get; }
        public bool IsChecked { set; get; }

        public int OrderIndex { set; get; }
        public bool IsShown { get; set; }
    }
    public class TestRestrictionModuleDTO
    {
        public TestRestrictionModuleDTO()
        {
            ListRoles = new List<TestRestrictionModuleRoleDTO>();
        }
        public int ModuleId { set; get; }

        public string ModuleName { set; get; }

        public string ModuleDisplayName { set; get; }

        public List<TestRestrictionModuleRoleDTO> ListRoles { set; get; }

        public int OrderIndex { set; get; }
    }

    public class TestRestrictionModuleMatrixDTO
    {
        public TestRestrictionModuleMatrixDTO()
        {

        }
        public int ModuleId { set; get; }

        public int RoleId { set; get; }
    }
    public class CategoryTestRestrictionModuleMatrixDto
    {
        public int XLITestRestrictionModuleRoleID { set; get; } = 0;
        public int RestrictedObjectCategoryTestId { set; get; }
        public int RoleId { set; get; }
        public bool AllowAccess { set; get; }
        public int DistrictId { set; get; }
        public string RestrictionTypeName { set; get; }
        public string Level { set; get; }
        public int RestrictionModuleID { get; set; }
    }
    public class GetTestRestrictionModuleRoleByBankAndDistrictReuqestDTO
    {
        public int RoleID { set; get; }

        public int BankID { set; get; }
        public string Rolename { set; get; }

        public string PublishedLevelName { set; get; }

        public int PublishedLevelID { set; get; }

        public string RestrictedObjectName { set; get; }

        public int RestrictedObjectID { set; get; }
        public int? UserDistrictID { get; set; }
    }

    public class SaveTestRestrictionModuleRequestDTO
    {
        public string PublishedLevelName { set; get; }

        public int PublishedLevelID { set; get; }

        public string RestrictedObjectName { set; get; }

        public int RestrictedObjectID { set; get; }

        public DateTime ModifiedDate { set; get; }

        public int ModifiedUser { set; get; }

        public List<TestRestrictionModuleDTO> ListTestRestrictionModules { set; get; }

        public List<TestRestrictionModuleMatrixDTO> ListTestRestrictionModulesRoleMatrix { set; get; }
        public int Roleuserid { set; get; }

    }
    public class SaveCategoriesTestsRestrictionModuleRequestDto
    {
        public int DistrictId { set; get; }
        public int RestrictionModuleId { set; get; }
        public int PublishedLevelID { set; get; }
        public string PublishedLevelName { set; get; }
        public string RestrictedObjectName { set; get; }
        public DateTime ModifiedDate { set; get; }
        public int ModifiedUser { set; get; }
        public List<CategoryTestRestrictionModuleMatrixDto> CategoriesTestsRestriction { set; get; }
    }

    public class DeleteAllRetrictBankTestFromBankIdAndDistrictRequestDTO
    {
        public DeleteAllRetrictBankTestFromBankIdAndDistrictRequestDTO()
        {
            ListVirtualTest = new List<int>();
        }
        public int BankID { set; get; }

        public int DistrictID { set; get; }

        public List<int> ListVirtualTest { set; get; }
    }

    public class CategoryRestrictionModuleDto
    {
        public int CategoryId { set; get; }
        public int XLITeacherModuleRoleId { get; set; }
        public string CategoryName { set; get; }
        public string TeacherRestriction { get; set; }
        public string SchoolAdminRestriction { get; set; }
        public int XLISchoolAdminModuleRoleId { get; set; }
        public string TeacherRestrictionDisplay { get; set; }
        public string SchoolAdminRestrictionDisplay { get; set; }
        public string TeacherRestrictionContent { get; set; }
        public string SchoolAdminRestrictionContent { get; set; }
        public string TestName { set; get; }
    }

    public class CategoryRestrictionModuleRoleDto
    {
        public int XLTTestRestrictionModuleRoleId { set; get; }
        public int RoleId { set; get; }
        public int RestrictedObjectID { set; get; }
        public int XLITestRestrictionModuleID { set; get; }
    }

    public class TestRestrictionDto
    {
        public int VirtualTestID { get; set; }
        public int CategoryId { set; get; }
        public string TestName { get; set; }
        public string CategoryName { set; get; }
        public string Subject { get; set; }
        public string Grade { get; set; }
        public string TeacherRestriction { get; set; }
        public string SchoolAdminRestriction { get; set; }
        public int XLITeacherModuleRoleId { get; set; }
        public int XLISchoolAdminModuleRoleId { get; set; }
        public string TeacherRestrictionContent { get; set; }
        public string SchoolAdminRestrictionContent { get; set; }
        public string TeacherRestrictionDisplay { get; set; }
        public string SchoolAdminRestrictionDisplay { get; set; }

    }
    public class TestRestrictionFilter: GenericDataTableRequest
    {
        public string GradeIds { get; set; }
        public string Level { get; set; } = PreferenceLevel.DISTRICT;
        public int DistrictId { get; set; }
        public string Subjects { get; set; }
        public string CategoryIds { get; set; }
    }
    public class TestRestrictionRequestDto: PaggingInfo
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public int DistrictID { get; set; }
        public string Level { get; set; }
        public string GradeIds { get; set; }
        public string Subjects { get; set; }       
        public string CategoryIds { get; set; }
        public int Visibilities { get; set; }
        public string GeneralSearch { get; set; }

    }
    public class TestForRetrictionResponseDto
    {
        public TestForRetrictionResponseDto()
        {
            Data = new List<TestRestrictionDto>();
        }
        public int TotalRecord { get; set; }
        public List<TestRestrictionDto> Data { get; set; }       
        
    }
    public class GenericDataTableRequest
    {
        public string sSortDir_0 { get; set; }
        public int iColumns { get; set; }
        public int iDisplayStart { get; set; }
        public int iDisplayLength { get; set; }
        public string sColumns { get; set; }
        public int? iSortCol_0 { get; set; }
        public int sEcho { get; set; }
        public string sSearch { get; set; }
    }
    public class ListItemRestriction
    {
        public ListItemRestriction() {}
        public ListItemRestriction(int id, string name) {
            Id = id;
            Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string TeacherRestriction { get; set; }
        public string SchoolAdminRestriction { get; set; }
        public string TeacherRestrictionContent { get; set; }
        public string SchoolAdminRestrictionContent { get; set; }
    }
    public class CategoryTestRestrictResponseDto
    {
        public string CategoryTeacherRestriction { get; set; }
        public string CategorySchoolRestriction { get; set; }
        public string TestTeacherRestriction { get; set; }
        public string TestSchoolRestriction { get; set; }
        public string CategoryTeacherRestrictionContent { get; set; }
        public string CategorySchoolRestrictionContent { get; set; }
        public string TestTeacherRestrictionContent { get; set; }
        public string TestSchoolRestrictionContent { get; set; }
    }

}
