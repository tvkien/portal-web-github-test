using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System;
using LinkIt.BubbleSheetPortal.Data.DBContextFakeData;
using LinkIt.BubbleSheetPortal.Models.RestrictionDTO;
using LinkIt.BubbleSheetPortal.Models.Enum;
using System.Text;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ITestRestrictionModuleRepository
    {
        List<TestRestrictionModuleDTO> GetTestRestrictionModuleRoleByBankAndDistrict
            (GetTestRestrictionModuleRoleByBankAndDistrictReuqestDTO
            getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO);
        void SaveTestRestrictionModule(SaveTestRestrictionModuleRequestDTO
            saveTestRestrictionModuleRequestDTO);
        void DeleteAllRetrictBankTestFromBankIdAndDistrict
            (DeleteAllRetrictBankTestFromBankIdAndDistrictRequestDTO
             DeleteAllRetrictBankTestFromBankIdAndDistrictRequestDTO);

        List<TestRestrictionModuleDTO>
           GetTestRestrictionModuleRoleByBankTestAndDistrict
           (GetTestRestrictionModuleRoleByBankAndDistrictReuqestDTO
            getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO);

        List<RestrictionDTO> GetListRestriction(string moduleCode, int userId, int roleId, PublishLevelTypeEnum publishLevelType, int publishLevelId);
        List<RestrictionDTO> GetListRestriction(int userId, int roleId, int districtId);
        void SaveRestrictionCategoryTest(SaveCategoriesTestsRestrictionModuleRequestDto saveCategoryTestRestrictionModuleRequestDTO);
        List<CategoryRestrictionModuleDto> GetCategoriesRestriction(int districtId, List<SelectListItemDTO> Categories);
        TestForRetrictionResponseDto GetTestsRestriction(TestRestrictionRequestDto criteria);
        CategoryTestRestrictResponseDto GetRestrictionByRestrictedObject(int categoryId, int virtualTestId, int districtId);
    }
    public class TestRestrictionModuleRepository : ITestRestrictionModuleRepository
    {
        private readonly Table<XLITestRestrictionModule> table;
        private readonly Table<XLITestRestrictionModuleRole> tableModuleRole;
        private readonly string[] ListRoleRetricts = { "NetworkAdmin", "Administrator", "School", "Teacher" };
        private readonly RestrictionDataContext dbContext;
        private readonly RetrictionDataContextFakeData dbContextFakedata;        
        public TestRestrictionModuleRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = RestrictionDataContext.Get(connectionString).GetTable<XLITestRestrictionModule>();
            tableModuleRole = RestrictionDataContext.Get(connectionString).GetTable<XLITestRestrictionModuleRole>();
            dbContext = RestrictionDataContext.Get(connectionString);
        }
        //for Unit Test
        public TestRestrictionModuleRepository()
        {
            var connectionString = System.Configuration.ConfigurationManager.
            ConnectionStrings["LinkItConnectionString"].ConnectionString;
            table = RestrictionDataContext.Get(connectionString).GetTable<XLITestRestrictionModule>();
            tableModuleRole = RestrictionDataContext.Get(connectionString).GetTable<XLITestRestrictionModuleRole>();
            if (!IsTest)
                dbContext = RestrictionDataContext.Get(connectionString);
            else
                dbContextFakedata = new RetrictionDataContextFakeData();
        }
        private bool IsTest
        {
            get
            {

                var istest = System.Configuration.ConfigurationManager.AppSettings["IsTest"];
                if (string.IsNullOrEmpty(istest))
                    return false;
                else
                    return istest.ToString() == "1";

            }
        }
        private int GetOrderIndexModuleRoleByName(string rolename)
        {

            int orderIndex = 0;
            switch (rolename)
            {
                case "RestrictAll":
                    orderIndex = 0;
                    break;
                case "NetworkAdmin":
                    orderIndex = 1;
                    break;
                case "Administrator":
                    orderIndex = 2;
                    break;
                case "School":
                    orderIndex = 3;
                    break;
                case "Teacher":
                    orderIndex = 4;
                    break;
            }

            return orderIndex;
        }

        private int GetOrderIndexModuleByName(string modulename)
        {
            int orderIndex = 0;
            switch (modulename)
            {
                case "print":
                    orderIndex = 0;
                    break;
                case "manager":
                    orderIndex = 2;
                    break;
                case "assign":
                    orderIndex = 1;
                    break;
                case "preview":
                    orderIndex = 3;
                    break;
                case "reporting":
                    orderIndex = 4;
                    break;
                case "review":
                    orderIndex = 5;
                    break;
                case "view_grade":
                    orderIndex = 6;
                    break;
            }

            return orderIndex;

        }

        private List<TestRestrictionModuleRoleDTO>
            GetTestRestrictionModuleRoleShowByRoleName(string rolename)
        {
            var listsystemroleretricts = new List<TestRestrictionModuleRoleDTO>();
            if (!IsTest)
                listsystemroleretricts = dbContext.Roles.
                    Where(x => ListRoleRetricts.Contains(x.Name)).ToList().Select(x =>
                      new TestRestrictionModuleRoleDTO
                      {
                          RoleID = x.RoleID,
                          RoleName = x.Name,
                          RoleDisplayName = GetModuleRoleNameDisplayFromName(x.Name),
                          OrderIndex = GetOrderIndexModuleRoleByName(x.Name)
                      }
                  ).ToList();
            else
                listsystemroleretricts = dbContextFakedata.Roles.Where
                    (x => ListRoleRetricts.Contains(x.Name)).ToList().Select(x =>
                   new TestRestrictionModuleRoleDTO
                   {
                       RoleID = x.RoleID,
                       RoleName = x.Name,
                       RoleDisplayName = GetModuleRoleNameDisplayFromName(x.Name),
                       OrderIndex = GetOrderIndexModuleRoleByName(x.Name)
                   }
              ).ToList();


            listsystemroleretricts.Insert(0, new TestRestrictionModuleRoleDTO
            {
                RoleID = 0,
                RoleDisplayName = "Restrict All",
                RoleName = "RestrictAll",
                OrderIndex = 0
            });

            switch (rolename.Trim().ToLower())
            {
                case "publisher":
                    return listsystemroleretricts;
                case "network admin":
                    listsystemroleretricts = listsystemroleretricts.Where(x => x.RoleID == 0 || x.RoleName.ToLower() != "networkadmin").ToList();
                    break;
                case "administrator":
                    listsystemroleretricts = listsystemroleretricts.Where(x => x.RoleID == 0 || (x.RoleName.ToLower() != "administrator" && x.RoleName.ToLower() != "networkadmin")).ToList();
                    break;
                case "school":
                    listsystemroleretricts = listsystemroleretricts.Where(x => x.RoleID == 0 || x.RoleName.ToLower() == "school").ToList();
                    break;
                default:
                    listsystemroleretricts = new List<TestRestrictionModuleRoleDTO>();
                    break;
            }

            return listsystemroleretricts;
        }
        private string GetModuleRoleNameDisplayFromName(string name)
        {
            string displayname = "";
            switch (name)
            {
                case "NetworkAdmin":
                    displayname = "Network Admin";
                    break;
                case "Administrator":
                    displayname = "District Admin";
                    break;
                case "School":
                    displayname = "School Admin";
                    break;
                case "Teacher":
                    displayname = "Teacher";
                    break;
            }

            return displayname;
        }

        public List<TestRestrictionModuleDTO>
            GetTestRestrictionModuleRoleByBankAndDistrict
            (GetTestRestrictionModuleRoleByBankAndDistrictReuqestDTO getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO)
        {
            var roleId = getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.RoleID;
            if (string.IsNullOrEmpty(getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.Rolename))
            {
                // entityrole = null;
                Entities.Role entityrole = null;
                if (!IsTest)
                    entityrole = dbContext.Roles.FirstOrDefault
                        (x => x.RoleID == roleId);
                else
                    entityrole = dbContextFakedata.Roles.FirstOrDefault(x => x.RoleID == roleId);

                getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.Rolename = entityrole.Name;
            }
            var listTestRestrictionModuleRoleDTO =
                GetTestRestrictionModuleRoleShowByRoleName
                (getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.Rolename);
            List<TestRestrictionModuleDTO> listTestRestrictionModule = null;
            if (!IsTest)
            {
                listTestRestrictionModule = dbContext.XLITestRestrictionModules.Where(x => x.Code != RestrictionConstant.Module_Reporting).Select(x => new TestRestrictionModuleDTO
                {
                    ModuleId = x.XLITestRestrictionModuleID,
                    ModuleName = x.Code,
                    ModuleDisplayName = x.Name,
                    ListRoles = listTestRestrictionModuleRoleDTO

                }).ToList();
            }
            else
            {
                listTestRestrictionModule = dbContextFakedata.XLITestRestrictionModules.Where(x => x.Code != RestrictionConstant.Module_Reporting).Select(x => new TestRestrictionModuleDTO
                {
                    ModuleId = x.XLITestRestrictionModuleID,
                    ModuleName = x.Code,
                    ModuleDisplayName = x.Name,
                    ListRoles = listTestRestrictionModuleRoleDTO

                }).ToList();

            }
            var listTestRestrictionModuleCondition =
                !IsTest ?
                dbContext.XLITestRestrictionModules.Select(x => new
                {
                    ModuleID = x.XLITestRestrictionModuleID,
                    ListRolesID = x.XLITestRestrictionModuleRoles.Where(y => y.RestrictedObjectName == getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.RestrictedObjectName
                    && y.RestrictedObjectID == getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.RestrictedObjectID
                    && y.PublishedLevelName == getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.PublishedLevelName
                    && y.PublishedLevelID == getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.PublishedLevelID).Select(y => y.RoleID).ToList()
                }).ToList()
             :
             dbContextFakedata.XLITestRestrictionModules.Select(x => new
             {
                 ModuleID = x.XLITestRestrictionModuleID,
                 ListRolesID = x.XLITestRestrictionModuleRoles.Where
                 (y => y.RestrictedObjectName == getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.RestrictedObjectName
                 && y.RestrictedObjectID == getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.RestrictedObjectID
                 && y.PublishedLevelName == getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.PublishedLevelName
                 && y.PublishedLevelID == getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.PublishedLevelID).
                 Select(y => y.RoleID).ToList()
             }).ToList()


             ;

            listTestRestrictionModule = (from ld in listTestRestrictionModule
                                         join lc in listTestRestrictionModuleCondition on ld.ModuleId equals lc.ModuleID into ldlc
                                         from lc in ldlc.DefaultIfEmpty()                                        

                                         select new TestRestrictionModuleDTO
                                         {
                                             ModuleId = ld.ModuleId,
                                             ModuleName = ld.ModuleName,
                                             ModuleDisplayName = ld.ModuleDisplayName,
                                             OrderIndex = GetOrderIndexModuleByName(ld.ModuleName),
                                             ListRoles = ld.ListRoles.Select(x => new TestRestrictionModuleRoleDTO
                                             {
                                                 RoleID = x.RoleID,
                                                 RoleDisplayName = x.RoleDisplayName,
                                                 RoleName = x.RoleName,
                                                 OrderIndex = x.OrderIndex,
                                                 IsChecked = lc == null || lc.ListRolesID == null || lc.ListRolesID.Count() == 0 ? false :
                                                 lc.ListRolesID.Contains(x.RoleID)

                                             }).ToList().OrderBy(x => x.OrderIndex).ToList()

                                         }).ToList().OrderBy(x => x.OrderIndex).ToList();
            var manageTest = listTestRestrictionModule.FirstOrDefault(m => m.ModuleName == TestRestrictionModuleConstant.ManageTest);
            if (manageTest != null)
            {
                // Don't show Manage Test for District Admin
                // Don't show Manage Test if the district of current user is not bank's district
                if (roleId == (int)Permissions.DistrictAdmin ||
                    getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.PublishedLevelID != getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.UserDistrictID)
                {
                    listTestRestrictionModule.Remove(manageTest);
                }

                // no need to restrict School Admin, Teacher by Publisher and Network Admin
                else if (roleId == (int)Permissions.Publisher || roleId == (int)Permissions.NetworkAdmin)
                {
                    var roles = manageTest.ListRoles.Where(r => r.RoleID == (int)Permissions.SchoolAdmin || r.RoleID == (int)Permissions.Teacher).ToList();
                    if (roles.Any())
                    {
                        roles.ForEach(r => { r.IsShown = false; });
                    }
                }
            }

            listTestRestrictionModule.ForEach(item =>
            {
                item.ListRoles.FirstOrDefault().IsChecked = item.ListRoles.Where(r => r.IsShown).All(x => x.IsChecked || x.RoleID == 0);
            });
            return listTestRestrictionModule;

        }

        public List<TestRestrictionModuleDTO>
           GetTestRestrictionModuleRoleByBankTestAndDistrict
           (GetTestRestrictionModuleRoleByBankAndDistrictReuqestDTO getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO)
        {
            var roleId = getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.RoleID;
            var listforbankretrict = GetTestRestrictionModuleRoleByBankAndDistrict(new GetTestRestrictionModuleRoleByBankAndDistrictReuqestDTO
            {
                PublishedLevelID = getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.PublishedLevelID,
                PublishedLevelName = getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.PublishedLevelName,
                RestrictedObjectID = getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO.BankID,
                RestrictedObjectName = "bank",
                RoleID = roleId
            });
            var listforbanktestretrict = GetTestRestrictionModuleRoleByBankAndDistrict(getTestRestrictionModuleRoleByBankAndDistrictReuqestDTO);
            listforbanktestretrict.ForEach(item =>
            {
                var bankretrict = listforbankretrict.Where(x => x.ModuleId == item.ModuleId).FirstOrDefault();
                if (bankretrict != null)
                {
                    item.ListRoles.ForEach(role =>
                    {
                        role.IsDisable = bankretrict.ListRoles.Any(x => x.RoleID == role.RoleID && x.IsChecked);
                        if (role.IsDisable)
                            role.IsChecked = bankretrict.ListRoles.Any(x => x.RoleID == role.RoleID && x.IsChecked);
                    });
                }
            });
            return listforbanktestretrict;
        }

        public void SaveTestRestrictionModule(SaveTestRestrictionModuleRequestDTO saveTestRestrictionModuleRequestDTO)
        {
            saveTestRestrictionModuleRequestDTO.ModifiedDate = DateTime.Now;

            Entities.Role entityrole = null;

            entityrole = dbContext.Roles.FirstOrDefault(x => x.RoleID == saveTestRestrictionModuleRequestDTO.Roleuserid);

            var rolename = entityrole.Name;
            var listroles = GetTestRestrictionModuleRoleShowByRoleName(rolename).Select(x => x.RoleID);

            var listTestRestrictionModuleRequestrole = dbContext.XLITestRestrictionModuleRoles.Where
                (x => x.PublishedLevelID == saveTestRestrictionModuleRequestDTO.PublishedLevelID
                &&
                x.PublishedLevelName == saveTestRestrictionModuleRequestDTO.PublishedLevelName
                &&
                x.RestrictedObjectID == saveTestRestrictionModuleRequestDTO.RestrictedObjectID
                 &&
                x.RestrictedObjectName == saveTestRestrictionModuleRequestDTO.RestrictedObjectName
                &&
                listroles.Contains(x.RoleID)
                );
            dbContext.XLITestRestrictionModuleRoles.DeleteAllOnSubmit(listTestRestrictionModuleRequestrole);
            if (saveTestRestrictionModuleRequestDTO.ListTestRestrictionModulesRoleMatrix != null
                && saveTestRestrictionModuleRequestDTO.ListTestRestrictionModulesRoleMatrix.Count > 0)
            {
                foreach (var testRestrictionModuleRoleMatrix in saveTestRestrictionModuleRequestDTO.

                    ListTestRestrictionModulesRoleMatrix.Where(x => x.RoleId != 0 && x.ModuleId != 0).ToList())
                {
                    dbContext.XLITestRestrictionModuleRoles.InsertOnSubmit(new XLITestRestrictionModuleRole
                    {
                        ModifiedDate = saveTestRestrictionModuleRequestDTO.ModifiedDate,
                        ModifiedUser = saveTestRestrictionModuleRequestDTO.ModifiedUser,
                        PublishedLevelID = saveTestRestrictionModuleRequestDTO.PublishedLevelID,
                        PublishedLevelName = saveTestRestrictionModuleRequestDTO.PublishedLevelName,
                        RestrictedObjectID = saveTestRestrictionModuleRequestDTO.RestrictedObjectID,
                        RestrictedObjectName = saveTestRestrictionModuleRequestDTO.RestrictedObjectName,
                        RoleID = testRestrictionModuleRoleMatrix.RoleId,
                        XLITestRestrictionModuleID = testRestrictionModuleRoleMatrix.ModuleId
                    });

                }
            };
            dbContext.SubmitChanges();

        }

        public void SaveRestrictionCategoryTest(SaveCategoriesTestsRestrictionModuleRequestDto saveCategoryTestRestrictionModuleRequestDTO)
        {
            foreach (var testRestrictionModule in saveCategoryTestRestrictionModuleRequestDTO.CategoriesTestsRestriction)
            {
                var entity = dbContext.XLITestRestrictionModuleRoles.FirstOrDefault(x =>
                    x.RestrictedObjectName == saveCategoryTestRestrictionModuleRequestDTO.RestrictedObjectName
                    && x.RoleID == testRestrictionModule.RoleId
                    && x.RestrictedObjectID == testRestrictionModule.RestrictedObjectCategoryTestId
                    && x.PublishedLevelID == saveCategoryTestRestrictionModuleRequestDTO.PublishedLevelID
                    && x.XLITestRestrictionModuleID == testRestrictionModule.RestrictionModuleID);

                if (testRestrictionModule.AllowAccess)
                {
                    if (entity != null)
                    {
                        dbContext.XLITestRestrictionModuleRoles.DeleteOnSubmit(entity);
                    }
                }
                else
                {
                    if (entity == null)
                    {
                        entity = new XLITestRestrictionModuleRole
                        {
                            PublishedLevelID = saveCategoryTestRestrictionModuleRequestDTO.PublishedLevelID,
                            PublishedLevelName = saveCategoryTestRestrictionModuleRequestDTO.PublishedLevelName,
                            XLITestRestrictionModuleID = testRestrictionModule.RestrictionModuleID,
                            ModifiedDate = DateTime.UtcNow,
                            ModifiedUser = saveCategoryTestRestrictionModuleRequestDTO.ModifiedUser,
                            RestrictedObjectName = saveCategoryTestRestrictionModuleRequestDTO.RestrictedObjectName,
                            RestrictedObjectID = testRestrictionModule.RestrictedObjectCategoryTestId,
                            RoleID = testRestrictionModule.RoleId
                        };

                        dbContext.XLITestRestrictionModuleRoles.InsertOnSubmit(entity);
                    }
                    else
                    {
                        entity.ModifiedDate = DateTime.UtcNow;
                        entity.ModifiedUser = saveCategoryTestRestrictionModuleRequestDTO.ModifiedUser;
                    }
                }

                dbContext.SubmitChanges();
            }
        }


        public void DeleteAllRetrictBankTestFromBankIdAndDistrict(DeleteAllRetrictBankTestFromBankIdAndDistrictRequestDTO
            deleteAllRetrictBankTestFromBankIdAndDistrictRequestDTO)
        {
            var listentitirsdelete = dbContext.XLITestRestrictionModuleRoles.Where
                (x =>
                //delete all retrictions bank and district
                (x.PublishedLevelName == "district" && x.PublishedLevelID == deleteAllRetrictBankTestFromBankIdAndDistrictRequestDTO.DistrictID
                && x.RestrictedObjectName == "bank" && x.RestrictedObjectID == deleteAllRetrictBankTestFromBankIdAndDistrictRequestDTO.BankID)

                ||
                //delete all retrictions virtualtest and district
                 (x.PublishedLevelName == "district" && x.PublishedLevelID == deleteAllRetrictBankTestFromBankIdAndDistrictRequestDTO.DistrictID
                && x.RestrictedObjectName == "test" && deleteAllRetrictBankTestFromBankIdAndDistrictRequestDTO.ListVirtualTest.Contains(x.RestrictedObjectID))
                 )
                ;
            dbContext.XLITestRestrictionModuleRoles.DeleteAllOnSubmit(
                listentitirsdelete);
            dbContext.SubmitChanges();
        }
        #region Author: Hiep Bui
        public List<RestrictionDTO> GetListRestriction(string moduleCode, int userId, int roleId, PublishLevelTypeEnum publishLevelType, int publishLevelId)
        {
            var module = dbContext.XLITestRestrictionModules.FirstOrDefault(m => m.Code.Equals(moduleCode));

            if (module == null)
            {
                throw new System.ArgumentException(string.Format("Can not find {0} module in database", moduleCode), "moduleCode");
            }

            var query = dbContext.XLITestRestrictionModuleRoles
                                        .Where(m => m.RoleID == roleId && m.XLITestRestrictionModuleID == module.XLITestRestrictionModuleID);

            var publishedLevelName = publishLevelType == PublishLevelTypeEnum.District ? RestrictionConstant.PublishedLevel_District : RestrictionConstant.PublishedLevel_School;
            query = query.Where(x => x.PublishedLevelName == publishedLevelName && x.PublishedLevelID == publishLevelId);

            var restrictionList = query
                                .Select(m => new RestrictionDTO
                                {
                                    Id = m.XLITestRestrictionModuleRoleID,
                                    PublishLevelType = m.PublishedLevelName.Equals(RestrictionConstant.PublishedLevel_District)
                                                            ? PublishLevelTypeEnum.District : PublishLevelTypeEnum.School,
                                    PublishLevelId = m.PublishedLevelID,
                                    TestRestrictionModuleId = m.XLITestRestrictionModuleID,
                                    RestrictionObjectType = m.RestrictedObjectName.Equals(RestrictionConstant.RestrictedObject_Bank)
                                                                ? RestrictionObjectType.Bank : RestrictionObjectType.Test,
                                    RestrictionObjectId = m.RestrictedObjectID,
                                    RoleId = m.RoleID,
                                    UserId = userId

                                }).ToList();

            return restrictionList;
        }

        public List<RestrictionDTO> GetListRestriction(int userId, int roleId, int districtId)
        {
            var modules = dbContext.XLITestRestrictionModules.ToList();

            var restrictionList = dbContext.XLITestRestrictionModuleRoles
                                        .Where(m => m.RoleID == roleId && m.PublishedLevelName == RestrictionConstant.PublishedLevel_District && m.PublishedLevelID == districtId)
                                       .AsEnumerable()
                                        .Select(m => new RestrictionDTO
                                        {
                                            Id = m.XLITestRestrictionModuleRoleID,
                                            PublishLevelType = PublishLevelTypeEnum.District,
                                            PublishLevelId = m.PublishedLevelID,
                                            TestRestrictionModuleId = m.XLITestRestrictionModuleID,
                                            TestRestrictionModuleCode = modules.FirstOrDefault(h => h.XLITestRestrictionModuleID == m.XLITestRestrictionModuleID).Code,
                                            RestrictionObjectType = m.RestrictedObjectName.Equals(RestrictionConstant.RestrictedObject_Bank)
                                                                        ? RestrictionObjectType.Bank : RestrictionObjectType.Test,
                                            RestrictionObjectId = m.RestrictedObjectID,
                                            RoleId = m.RoleID,
                                            UserId = userId
                                        }).ToList();

            return restrictionList;
        }
        public List<CategoryRestrictionModuleDto> GetCategoriesRestriction(int districtId, List<SelectListItemDTO> categories)
        {
            var results = new List<CategoryRestrictionModuleDto>();
            var categoriesRestricteds = dbContext.XLITestRestrictionModuleRoles
                    .Where(x => x.PublishedLevelID == districtId && x.RestrictedObjectName == Constanst.CATEGORY && (x.XLITestRestrictionModuleID == 9 || x.XLITestRestrictionModuleID == 15))
                    .Select(x => new CategoryRestrictionModuleRoleDto
                    {
                        XLTTestRestrictionModuleRoleId = x.XLITestRestrictionModuleRoleID,
                        RestrictedObjectID = x.RestrictedObjectID,
                        RoleId = x.RoleID,
                        XLITestRestrictionModuleID = x.XLITestRestrictionModuleID,
                    }).ToList();
            
            foreach (var item in categories)
            {                
                var teacherRestriction = categoriesRestricteds.FirstOrDefault(x => x.RestrictedObjectID == item.Id && x.RoleId == (int)RoleEnum.Teacher && x.XLITestRestrictionModuleID == 9);
                var schoolRestriction = categoriesRestricteds.FirstOrDefault(x => x.RestrictedObjectID == item.Id && x.RoleId == (int)RoleEnum.SchoolAdmin && x.XLITestRestrictionModuleID == 9);

                var teacherRestrictionContent = categoriesRestricteds.FirstOrDefault(x => x.RestrictedObjectID == item.Id && x.RoleId == (int)RoleEnum.Teacher && x.XLITestRestrictionModuleID == 15);
                var schoolRestrictionContent = categoriesRestricteds.FirstOrDefault(x => x.RestrictedObjectID == item.Id && x.RoleId == (int)RoleEnum.SchoolAdmin && x.XLITestRestrictionModuleID == 15);

                var schoolAdminRestrictionDisplay = Constanst.FULLACCESS;
                var teacherRestrictionDisplay = Constanst.FULLACCESS;
                if (teacherRestriction != null)
                {
                    teacherRestrictionDisplay = Constanst.NOACCESS;
                }
                else if (teacherRestriction == null && teacherRestrictionContent != null)
                {
                    teacherRestrictionDisplay = Constanst.PARTIALACCESS;
                }

                if (schoolRestriction != null)
                {
                    schoolAdminRestrictionDisplay = Constanst.NOACCESS;
                }
                else if (schoolRestriction == null && schoolRestrictionContent != null)
                {
                    schoolAdminRestrictionDisplay = Constanst.PARTIALACCESS;
                }

                results.Add(new CategoryRestrictionModuleDto
                {
                    CategoryId = item.Id,
                    CategoryName = item.Name,
                    XLITeacherModuleRoleId = teacherRestriction != null ? teacherRestriction.XLTTestRestrictionModuleRoleId : 0,
                    TeacherRestriction = teacherRestriction != null ? Constanst.NOACCESS : Constanst.FULLACCESS,
                    XLISchoolAdminModuleRoleId = schoolRestriction != null ? schoolRestriction.XLTTestRestrictionModuleRoleId : 0,
                    SchoolAdminRestriction = schoolRestriction != null ? Constanst.NOACCESS : Constanst.FULLACCESS,
                    TeacherRestrictionContent = teacherRestrictionContent == null && teacherRestriction == null ? Constanst.FULLACCESS : Constanst.NOACCESS,
                    SchoolAdminRestrictionContent = schoolRestrictionContent == null && schoolRestriction == null ? Constanst.FULLACCESS : Constanst.NOACCESS,
                    TeacherRestrictionDisplay = teacherRestrictionDisplay,
                    SchoolAdminRestrictionDisplay = schoolAdminRestrictionDisplay,
                });
            }

            return results;
        }

        public TestForRetrictionResponseDto GetTestsRestriction(TestRestrictionRequestDto criteria)
        {
            var result = new TestForRetrictionResponseDto();
            int? totalRecord = 0;
            var data = dbContext.GetTestRestriction(criteria.DistrictID, criteria.UserID, criteria.RoleID, criteria.CategoryIds,
                criteria.GradeIds, criteria.Subjects, criteria.StartRow, criteria.PageSize, criteria.GeneralSearch, criteria.SortColumn,
                criteria.SortDirection, ref totalRecord);
            
            result.Data.AddRange(data.Select(x => new TestRestrictionDto
            {
                VirtualTestID = x.VirtualTestID,
                TestName = x.TestName,
                CategoryId = x.CategoryID,
                CategoryName = x.CategoryName,
                Subject = x.SubjectName,
                Grade = x.GradeName,
                TeacherRestriction = x.TeacherRestriction,
                SchoolAdminRestriction = x.SchoolAdminRestriction,
                SchoolAdminRestrictionContent = x.SchoolAdminRestrictionContent,
                TeacherRestrictionContent = x.TeacherRestrictionContent,
                SchoolAdminRestrictionDisplay = x.SchoolAdminRestrictionDisplay,
                TeacherRestrictionDisplay = x.TeacherRestrictionDisplay
            }).DistinctBy(i=>i.VirtualTestID).ToList());
            result.TotalRecord = totalRecord ?? 0;
            return result;    
        } 

        public CategoryTestRestrictResponseDto GetRestrictionByRestrictedObject(int categoryId, int virtualTestId, int districtId)
        {
            var result = new CategoryTestRestrictResponseDto()
            {
                CategorySchoolRestriction = Constanst.FULLACCESS,
                CategoryTeacherRestriction = Constanst.FULLACCESS,
                TestSchoolRestriction = Constanst.FULLACCESS,
                TestTeacherRestriction = Constanst.FULLACCESS,
                CategorySchoolRestrictionContent = Constanst.FULLACCESS,
                CategoryTeacherRestrictionContent = Constanst.FULLACCESS,
                TestSchoolRestrictionContent = Constanst.FULLACCESS,
                TestTeacherRestrictionContent = Constanst.FULLACCESS,
            };
            var data = dbContext.XLITestRestrictionModuleRoles.Where(m => m.PublishedLevelID == districtId
                        && (m.RestrictedObjectID == categoryId || m.RestrictedObjectID == virtualTestId)).ToList();

            if (data != null && data.Any())
            {
                var categoryTeacherRestriction = data.Any(m => m.RestrictedObjectName == Constanst.CATEGORY && m.RoleID == (int)RoleEnum.Teacher && m.XLITestRestrictionModuleID == 9);
                var categorySchoolRestriction = data.Any(m => m.RestrictedObjectName == Constanst.CATEGORY && m.RoleID == (int)RoleEnum.SchoolAdmin && m.XLITestRestrictionModuleID == 9);
                var categoryTeacherRestrictionContent = data.Any(m => m.RestrictedObjectName == Constanst.CATEGORY && m.RoleID == (int)RoleEnum.Teacher && m.XLITestRestrictionModuleID == 15);
                var categorySchoolRestrictionContent = data.Any(m => m.RestrictedObjectName == Constanst.CATEGORY && m.RoleID == (int)RoleEnum.SchoolAdmin && m.XLITestRestrictionModuleID == 15);
                var testTeacherRestriction = data.Any(m => m.RestrictedObjectName == Constanst.TEST && m.RoleID == (int)RoleEnum.Teacher && m.XLITestRestrictionModuleID == 9);
                var testTeacherRestrictionContent = data.Any(m => m.RestrictedObjectName == Constanst.TEST && m.RoleID == (int)RoleEnum.Teacher && m.XLITestRestrictionModuleID == 15);
                var testSchoolRestriction = data.Any(m => m.RestrictedObjectName == Constanst.TEST && m.RoleID == (int)RoleEnum.SchoolAdmin && m.XLITestRestrictionModuleID == 9);
                var testSchoolRestrictionContent = data.Any(m => m.RestrictedObjectName == Constanst.TEST && m.RoleID == (int)RoleEnum.SchoolAdmin && m.XLITestRestrictionModuleID == 15);

                if (testTeacherRestrictionContent)
                {
                    result.TestTeacherRestrictionContent = Constanst.NOACCESS;
                }
                if (testSchoolRestrictionContent)
                {
                    result.TestSchoolRestrictionContent = Constanst.NOACCESS;
                }
                if (testTeacherRestriction)
                {
                    result.TestTeacherRestriction = Constanst.NOACCESS;
                    result.TestTeacherRestrictionContent = Constanst.NOACCESS;
                }
                if (testSchoolRestriction)
                {
                    result.TestSchoolRestriction = Constanst.NOACCESS;
                    result.TestSchoolRestrictionContent = Constanst.NOACCESS;
                }
                if (categoryTeacherRestrictionContent)
                {
                    result.CategoryTeacherRestrictionContent = Constanst.NOACCESS;
                    result.TestTeacherRestrictionContent = Constanst.NOACCESS;
                }
                if (categorySchoolRestrictionContent)
                {
                    result.CategorySchoolRestrictionContent = Constanst.NOACCESS;
                    result.TestSchoolRestrictionContent = Constanst.NOACCESS;
                }
                if (categoryTeacherRestriction)
                {
                    result.CategoryTeacherRestriction = Constanst.NOACCESS;
                    result.CategoryTeacherRestrictionContent = Constanst.NOACCESS;
                    result.TestTeacherRestriction = Constanst.NOACCESS;
                    result.TestTeacherRestrictionContent = Constanst.NOACCESS;
                }
                if (categorySchoolRestriction)
                {
                    result.CategorySchoolRestriction = Constanst.NOACCESS;
                    result.CategorySchoolRestrictionContent = Constanst.NOACCESS;
                    result.TestSchoolRestriction = Constanst.NOACCESS;
                    result.TestSchoolRestrictionContent = Constanst.NOACCESS;
                }
            }

            return result;
        }
        #endregion
    }
}
