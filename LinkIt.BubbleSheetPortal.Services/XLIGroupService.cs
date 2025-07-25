using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserGroup;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.Old.UserGroup;
using System;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class XLIGroupService
    {
        private readonly IXLIGroupRepository _xliGroupRepo;
        private readonly IXLIGroupUserRepository _xliGroupUserRepository;
        private readonly XLIAreaGroupRepository _xliAreaGroupRepo;
        private readonly XLIAreaGroupModuleRepository _xliAreaGroupModuleRepo;
        private readonly XLIAreaGMRoleRepository _xliAreaGMRoleRepo;
        private readonly IUserManageRepository _userManageRepository;
        private readonly IXLIIconAccessRepository _repositoryIconAccess;
        private readonly IXLIModuleAccessRepository _repositoryModuleAccess;
        private readonly IReadOnlyRepository<XliArea> _repositoryXliArea;
        private readonly IReadOnlyRepository<XliModule> _repositoryXliModule;
        private readonly IReadOnlyRepository<Configuration> _repositoryConfiguration;

        private readonly List<string> AreaCodeSupport = new List<string>
        {
            ContaintUtil.Home,
            ContaintUtil.Reporting,
            ContaintUtil.InterventionManager,
            ContaintUtil.Testdesign,
            ContaintUtil.Managebubblesheets,
            ContaintUtil.Onlinetesting,
            ContaintUtil.ResultsEntryDataLocker,
            ContaintUtil.SurveyModule,
            ContaintUtil.Testmanagement,
            ContaintUtil.Lessons,
            ContaintUtil.DataAdmin,
            ContaintUtil.Help,
            ContaintUtil.Techsupport //
        };

        private readonly List<string> ModuleCodeSupport = new List<string>
        {
            ContaintUtil.HomeItem,
            ContaintUtil.ReportItemACTReport,
            ContaintUtil.ReportingDownloadReport,
            ContaintUtil.ReportItemChytenReport,
            ContaintUtil.ReportingItemVueJS,
            ContaintUtil.ReportingItemCS,
            ContaintUtil.ReportingItemNew,
            ContaintUtil.DataExplorer,
            ContaintUtil.ReportItemSGOManager,
            ContaintUtil.ReportItemTLDSManager,
            ContaintUtil.NavigatorReportUpload,
            ContaintUtil.NavigatorReport,
            ContaintUtil.AblesReport, // => End Reporting Area
            ContaintUtil.IMDashboard,
            ContaintUtil.PerformanceCriteria,
            ContaintUtil.GroupingModel, // => End InterventionManager Area
            ContaintUtil.TestdesignAssessmentItemHTML,
            ContaintUtil.TestCloneItemBanks,
            ContaintUtil.TestdesignManageTest,
            ContaintUtil.TestdesignPassageNew,
            ContaintUtil.TestdesignTags,
            ContaintUtil.TestdesignAssementOld,
            ContaintUtil.TestdesignPassagesOld,
            ContaintUtil.TestdesignTestsOld,
            ContaintUtil.TestdesignRubric,
            ContaintUtil.ItemLibraryManagement,
            ContaintUtil.TestdesignManageAuthorGroup,
            ContaintUtil.TestdesignUpload3pItemBank, //Testdesign
            ContaintUtil.ManagebubblesheetsCreate,
            ContaintUtil.ManagebubblesheetsGrade,
            ContaintUtil.ManagebubblesheetsReview,
            ContaintUtil.ManagebubblesheetsError,
            ContaintUtil.ManagebubblesheetsGimport,//Managebubblesheets
            ContaintUtil.OnlinetestingItem,
            ContaintUtil.OnlineTestAssignmentRewrite,
            ContaintUtil.OnlineTestAssignmentReview,
            ContaintUtil.OnlineTestMonitorTestTaking,
            ContaintUtil.OnlinetestPreference,
            ContaintUtil.OnlinetestLockUnlockBank,//Onlinetesting
            ContaintUtil.Definetemplates,
            ContaintUtil.BuildEntryForms,
            ContaintUtil.EnterResults,//ResultsEntryDataLocker
            ContaintUtil.ManageSurveys,
            ContaintUtil.AssignSurveys,
            ContaintUtil.ReviewSurveys,
            ContaintUtil.TakeSurveys,//SurveyModule
            //ContaintUtil.TmgmtEinstructionimport,
            ContaintUtil.TmgmtTestresultremover,
            ContaintUtil.TmgmtTestregrader,
            ContaintUtil.TmgmtPurgetest,
            ContaintUtil.TmgmtPrinttest,
            //ContaintUtil.TmgmtCustomAssessments,
            ContaintUtil.TmgmtExtractTestResult,
            ContaintUtil.TmgmtTestResultTransfer,
            ContaintUtil.TmgmtTestResultExportGenesis,//Testmanagement
            ContaintUtil.TmgmtDefineTemplates,
            ContaintUtil.LearningLibrarySearch,
            ContaintUtil.LearningLibraryResourceAdmin,//Lessons
            ContaintUtil.DadManageuser,
            ContaintUtil.DadManageSchools,
            ContaintUtil.DadManageClasses,
            ContaintUtil.StudentLookup,
            ContaintUtil.ManageParent,
            ContaintUtil.DadManageRegisterClasses,
            ContaintUtil.DadManageRosters,
            ContaintUtil.DadLumos,
            ContaintUtil.DadManageProgram,
            ContaintUtil.Settings,
            ContaintUtil.SettingItem,
            ContaintUtil.DadManageUserGroup,
            ContaintUtil.OnlinetestStudentPreference,//DataAdmin
            ContaintUtil.HelpResource,
            ContaintUtil.UploadHelp, // Only Publisher
            ContaintUtil.HelpGuide,
            ContaintUtil.HelpTeachSupport,
            ContaintUtil.HelpIntroduction,
            ContaintUtil.CustomHelp,
            ContaintUtil.HelpVideotutorials,//Help
            ContaintUtil.TechUserImpersonation,
            //ContaintUtil.TechAPILog,
            ContaintUtil.DadManageLAC,
            //ContaintUtil.DadDistrictReferencedata,//Techsupport
            ContaintUtil.DataAccessManagement,
            ContaintUtil.DadManageSharingGroups,
            ContaintUtil.DadSecuritySettings
        };

        public XLIGroupService(
            IXLIGroupRepository repository,
            IXLIGroupUserRepository xliGroupUserRepository,
            IUserManageRepository userManageRepository,
            IXLIIconAccessRepository repositoryIconAccess,
            IXLIModuleAccessRepository repositoryModuleAccess,
            IReadOnlyRepository<XliArea> repositoryXliArea,
            IReadOnlyRepository<XliModule> repositoryXliModule,
            XLIAreaGroupRepository xliAreaGroupRepo,
            XLIAreaGroupModuleRepository xliAreaGroupModuleRepo,
            XLIAreaGMRoleRepository xliAreaGMRoleRepo,
            IReadOnlyRepository<Configuration> repositoryConfiguration)
        {
            _xliGroupRepo = repository;
            _xliGroupUserRepository = xliGroupUserRepository;
            _xliAreaGroupRepo = xliAreaGroupRepo;
            _xliAreaGroupModuleRepo = xliAreaGroupModuleRepo;
            _xliAreaGMRoleRepo = xliAreaGMRoleRepo;
            _userManageRepository = userManageRepository;
            _repositoryIconAccess = repositoryIconAccess;
            _repositoryModuleAccess = repositoryModuleAccess;
            _repositoryXliArea = repositoryXliArea;
            _repositoryXliModule = repositoryXliModule;
            _repositoryConfiguration = repositoryConfiguration;
        }

        public XLIGroup GetGroupByID(int groupID)
        {
            return _xliGroupRepo.Select().FirstOrDefault(x => x.XLIGroupID == groupID);
        }

        public IQueryable<XLIGroup> GetGroupByDistrict(int districtID)
        {
            return _xliGroupRepo.Select().Where(x => x.DistrictID.HasValue && x.DistrictID == districtID);
        }

        public GetUserGroupResponse GetUserGroupsByDistrict(GetUserGroupManagementPaginationRequest request)
        {
            var query = _xliGroupRepo.Select().Where(x => x.DistrictID.HasValue && x.DistrictID == request.DistrictID);

            if (!string.IsNullOrEmpty(request.GeneralSearch))
            {
                query = query.Where(x => x.Name.Contains(request.GeneralSearch));
            }

            if (!string.IsNullOrEmpty(request.SortColumn) && !string.IsNullOrEmpty(request.SortDirection))
            {
                switch (request.SortColumn)
                {
                    case nameof(XLIGroup.Name):
                        query = request.SortDirection.Equals("ASC") ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
                        break;

                    case nameof(XLIGroup.InheritRoleFunctionality):
                        query = request.SortDirection.Equals("ASC") ? query.OrderBy(x => x.InheritRoleFunctionality) : query.OrderByDescending(x => x.InheritRoleFunctionality);
                        break;
                }    
            }

            return new GetUserGroupResponse
            {
                TotalRecord = query.Count(),
                Data = query.Page(request.PageSize, request.StartRow).ToList()
            };
        }

        public bool RemoveUserFromGroup(int userID, int groupId)
        {
            return _userManageRepository.RemoveUserFromGroup(userID, groupId);
        }

        public void AddUsersToGroup(string userIDs, int groupID)
        {
            _xliGroupUserRepository.RemoveUsersFromGroup(userIDs.Split(';').Select(x => Convert.ToInt32(x)).ToList());
            _userManageRepository.AddUsersToGroup(userIDs, groupID);
        }

        public void AddXLIGroup(XLIGroupDto model)
        {
            _xliGroupRepo.Add(new XLIGroup
            {
                Name = model.Name,
                DistrictID = model.DistrictID,
                InheritRoleFunctionality = model.InheritRoleFunctionality
            });
        }

        public XLIGroupDto GetById(int xliGroupId, int districtID)
        {
            var entity = _xliGroupRepo.Select().FirstOrDefault(x => x.XLIGroupID == xliGroupId && x.DistrictID == districtID);

            if (entity == null)
            {
                return null;
            }

            return new XLIGroupDto
            {
                XLIGroupID = entity.XLIGroupID,
                DistrictID = entity.DistrictID,
                InheritRoleFunctionality = entity.InheritRoleFunctionality,
                Name = entity.Name
            };
        }

        public bool UpdateXLIGroup(XLIGroupDto model)
        {
            return _xliGroupRepo.Update(new XLIGroup
            {
                XLIGroupID = model.XLIGroupID,
                Name = model.Name,
                DistrictID = model.DistrictID,
                InheritRoleFunctionality = model.InheritRoleFunctionality
            });
        }

        public bool DeleteXLIGroup(int xliGroupId, int districtID)
        {
            return _xliGroupRepo.Delete(xliGroupId, districtID);
        }

        public bool IsExistGroupName(string groupName, int districtID, int xliGroupId = 0)
        {
            var entity = _xliGroupRepo.Select().FirstOrDefault(x => x.Name == groupName && x.DistrictID == districtID && x.XLIGroupID != xliGroupId);
            return entity != null;
        }

        public GetModuleAccessResponse GetModuleAccessByUser(GetModuleAccessPaginationRequest request)
        {
            var moduleIds = string.Join(",", GetXliModuleIDs(request));
            var allModules = _repositoryModuleAccess.GetXLIModuleAccessesByGroupID(request.XLIGroupID.Value, moduleIds).ToList();
            var modules = allModules;

            Func<XLIModuleAccessDto, bool> searchCondition = default;
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                searchCondition = x => x.ModuleName.ToLower().Contains(request.SearchString.ToLower())
                                        || x.AreaName.ToLower().Contains(request.SearchString.ToLower())
                                        || x.DistrictAccess.ToLower().Contains(request.SearchString.ToLower())
                                        || x.UserGroupAccess.ToLower().Contains(request.SearchString.ToLower())
                                        || x.SchoolAccess.ToLower().Contains(request.SearchString.ToLower())
                                        || x.CurrentAccess.ToLower().Contains(request.SearchString.ToLower());

                modules = modules.Where(searchCondition).ToList();
            }

            var reportingGroupCodes = new string[] { ContaintUtil.ReportingItemVueJS, ContaintUtil.ReportingItemNew, ContaintUtil.ReportingItemCS, ContaintUtil.DataExplorer };
            var psuedoModuleIds = InsertPseudoReportingModules(allModules, modules, reportingGroupCodes);

            if (!string.IsNullOrEmpty(request.SortColumn) && !string.IsNullOrEmpty(request.SortDirection))
            {
                switch (request.SortColumn)
                {
                    case "Module":
                        modules = request.SortDirection.Equals("ASC")
                            ? modules.OrderBy(x => x.ModuleName).ToList()
                            : modules.OrderByDescending(x => x.ModuleName).ToList();
                        break;

                    case "Area":
                        modules = request.SortDirection.Equals("ASC")
                            ? modules.OrderBy(x => x.AreaName).ThenBy(x => x.ModuleName).ToList()
                            : modules.OrderByDescending(x => x.AreaName).ThenBy(x => x.ModuleName).ToList();
                        break;

                    default:
                        modules = modules.OrderBy(x => x.AreaName).ThenBy(x => x.ModuleName).ToList();
                        break;
                }
            }

            var result = AppendReportingModules(request.XLIGroupID.Value, modules, searchCondition, reportingGroupCodes);

            // Remove psuedo reporting modules
            if (psuedoModuleIds.Any())
                result.RemoveAll(x => psuedoModuleIds.Contains(x.ModuleID));

            int count = result.Count;
            result = result.Skip(request.StartRow).Take(request.PageSize).ToList();

            return new GetModuleAccessResponse
            {
                Data = result,
                TotalRecord = count
            };
        }

        private int[] InsertPseudoReportingModules(List<XLIModuleAccessDto> allModules, List<XLIModuleAccessDto> resultModules, string[] reportingGroupCodes)
        {
            var notExistedCodes = reportingGroupCodes.Where(x => !resultModules.Select(m => m.ModuleCode).Contains(x));
            if (notExistedCodes.Any())
            {
                var modules = allModules.Where(x => notExistedCodes.Contains(x.ModuleCode)).ToArray();
                resultModules.AddRange(modules);

                return modules.Select(x => x.ModuleID).ToArray();
            }

            return Array.Empty<int>();
        }

        private IEnumerable<GetReportingModulesDto> GetAvailableReportingModules()
        {
            var environment = System.Configuration.ConfigurationManager.AppSettings["Env"];
            if (string.IsNullOrEmpty(environment))
            {
                return Enumerable.Empty<GetReportingModulesDto>();
            }

            return _repositoryModuleAccess.GetReportingModules(environment)
                .Where(x=>x.ModuleCode != ContaintUtil.DataExplorer);
        }

        private List<GetModuleAccessDataDto> AppendReportingModules(int groupId, List<XLIModuleAccessDto> inputModules, Func<XLIModuleAccessDto, bool> searchCondition, string[] reportingGroupCodes)
        {
            var resultModules = inputModules.Select(x => new GetModuleAccessDataDto(x, default, default)).ToList();

            var availableReportingModules = GetAvailableReportingModules();
            if (!availableReportingModules.Any())
                return resultModules;

            var accessibleReportingModules = _repositoryModuleAccess
                .GetXLIModuleAccessesByGroupID(groupId, string.Join("," , availableReportingModules.Select(x => x.ModuleID)))
                .Join(availableReportingModules, ma => ma.ModuleID, m => m.ModuleID, (ma, m) => new { ma, m })
                .OrderBy(x => x.m.Order)
                .Select(x => x.ma)
                .ToList();

            if (searchCondition != default)
                accessibleReportingModules = accessibleReportingModules.Where(searchCondition).ToList();

            if (accessibleReportingModules.Any())
            {
                var reportingAreaName = resultModules.Find(x => reportingGroupCodes.Contains(x.ModuleCode))?.AreaName;
                if (!string.IsNullOrEmpty(reportingAreaName))
                    accessibleReportingModules.ForEach(x => { x.AreaName = reportingAreaName; });
            }

            var groups = new string[] { ContaintUtil.ReportingItemVueJS, ContaintUtil.DataExplorer };
            foreach (var groupCode in reportingGroupCodes)
            {
                var index = resultModules.FindIndex(x => x.ModuleCode == groupCode);
                if (index != -1)
                {
                    var item = resultModules[index];
                    item.GroupId = item.ModuleID;
                    item.GroupName = item.ModuleName;
                    resultModules.RemoveAt(index);
                    resultModules.Add(item);

                    if (accessibleReportingModules.Any())
                        resultModules.AddRange(accessibleReportingModules
                            .Where(x => groups.Contains(groupCode) || x.ModuleCode != ContaintUtil.UsageDashboard)
                            .Select(x => new GetModuleAccessDataDto(x, item.GroupId, item.GroupName)));
                }
            }

            return resultModules;
        }

        public GetModuleAccessSumaryResponse GetModuleAccessSumary(GetModuleAccessPaginationRequest request)
        {
            var moduleIds = GetXliModuleIDs(request);
            var availableReportingModules = GetAvailableReportingModules();
            var availableReportingModuleIds = availableReportingModules.Select(x => x.ModuleID);
            if (availableReportingModules.Any())
                moduleIds = moduleIds.Concat(availableReportingModuleIds);
            var result = _repositoryModuleAccess.GetModuleSumaryAccess(request.DistrictID.Value, string.Join(",", moduleIds)).ToList();

            RemoveReportingModulesIfNotAnyDashboard(result, availableReportingModuleIds);
            ReplaceArHTMLAreaByReporting(result, availableReportingModuleIds);

            if (!string.IsNullOrEmpty(request.SearchString))
            {
                result = result.Where(x => x.ModuleName.ToLower().Contains(request.SearchString.ToLower())
                                        || x.AreaName.ToLower().Contains(request.SearchString.ToLower())
                                        || x.DistrictAccess.ToLower().Contains(request.SearchString.ToLower())
                                        || x.SchoolAccess.ToLower().Contains(request.SearchString.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(request.SortColumn) && !string.IsNullOrEmpty(request.SortDirection))
            {
                switch (request.SortColumn)
                {
                    case "Module":
                        result = request.SortDirection.Equals("ASC")
                            ? result.OrderBy(x => x.ModuleName).ToList()
                            : result.OrderByDescending(x => x.ModuleName).ToList();
                        break;

                    case "Area":
                        result = request.SortDirection.Equals("ASC")
                            ? result.OrderBy(x => x.AreaName).ThenBy(x => x.ModuleName).ToList()
                            : result.OrderByDescending(x => x.AreaName).ThenBy(x => x.ModuleName).ToList();
                        break;

                    default:
                        result = result.OrderBy(x => x.AreaName).ThenBy(x => x.ModuleName).ToList();
                        break;
                }
            }

            int count = result.Count();
            result = result.Skip(request.StartRow).Take(request.PageSize).ToList();

            return new GetModuleAccessSumaryResponse
            {
                Data = result,
                TotalRecord = count
            };
        }

        private void RemoveReportingModulesIfNotAnyDashboard(List<XLIModuleAccessSummaryDto> modules, IEnumerable<int> reportingConfigModuleIds)
        {
            if (!modules.Any(y => y.ModuleCode == ContaintUtil.ReportingItemVueJS || y.ModuleCode == ContaintUtil.DataExplorer))
                modules.RemoveAll(x => x.ModuleCode == ContaintUtil.UsageDashboard);
            if (!modules.Any(y => y.ModuleCode == ContaintUtil.DataExplorer))
                modules.RemoveAll(x => x.ModuleCode == ContaintUtil.DataExplorer);
            var reportingDashboardCodes = new string[] { ContaintUtil.ReportingItemVueJS, ContaintUtil.ReportingItemNew, ContaintUtil.ReportingItemCS, ContaintUtil.DataExplorer };
            if (!modules.Any(x => reportingDashboardCodes.Contains(x.ModuleCode)))
                modules.RemoveAll(x => reportingConfigModuleIds.Contains(x.ModuleID));
        }

        private void ReplaceArHTMLAreaByReporting(List<XLIModuleAccessSummaryDto> modules, IEnumerable<int> reportingConfigModuleIds)
        {
            string reportingAreaName;
            var reportingArea = modules.FirstOrDefault(x => x.AreaCode == ContaintUtil.Reporting);
            if (reportingArea == null)
                reportingAreaName = _repositoryXliArea.Select().FirstOrDefault(x => x.Code == ContaintUtil.Reporting)?.DisplayName;
            else
                reportingAreaName = reportingArea.AreaName;

            var reportingModules = modules.Where(x => reportingConfigModuleIds.Contains(x.ModuleID)).ToList();
            reportingModules.ForEach(x =>
            {
                x.AreaName = reportingAreaName;
            });
        }

        public IEnumerable<XliArea> GetAreasByUser(User user, int districtID)
        {
            var areaIds = _repositoryIconAccess.GetAllXliAreasByUserId(user.Id, districtID, user.RoleId).Select(x => x.XliAreaId);
            return _repositoryXliArea.Select().Where(x => areaIds.Contains(x.XliAreaId) && AreaCodeSupport.Contains(x.Code)).ToList();
        }

        public IEnumerable<XliModule> GetModulesByArea(User user, int areaID)
        {
            var moduleIds = GetXliModuleIDs(new GetModuleAccessPaginationRequest
            {
                UserID = user.Id,
                DistrictID = user.DistrictId.Value,
                RoleID = user.RoleId,
                XLIAreaID = areaID
            });

            return _repositoryXliModule.Select().Where(x => moduleIds.Contains(x.XliModuleId) && ModuleCodeSupport.Contains(x.Code)).ToList();
        }

        public void AddModulePermission(AddModulePermissionRequest request)
        {
            var xliAreaGroup = new XLIAreaGroupDto
            {
                XLIAreaID = request.XLIAreaId,
                XLIGroupID = request.XLIGroupId
            };

            _xliAreaGroupRepo.Save(xliAreaGroup);

            var xliAreaGroupModule = new XLIAreaGroupModuleDto
            {
                XLIAreaGroupID = xliAreaGroup.XLIAreaGroupID,
                XLIModuleID = request.XLIModuleId,
                AllRoles = request.AllRoles
            };

            _xliAreaGroupModuleRepo.Save(xliAreaGroupModule);

            if (!request.AllRoles)
            {
                var gmRoles = new List<XLIAreaGMRoleDto>();

                if (request.NetworkAdmin)
                {
                    gmRoles.Add(new XLIAreaGMRoleDto
                    {
                        RoleID = (int)RoleEnum.NetworkAdmin,
                        XLIAreaGroupModuleID = xliAreaGroupModule.XLIAreaGroupModuleID
                    });
                }

                if (request.DistrictAdmin)
                {
                    gmRoles.Add(new XLIAreaGMRoleDto
                    {
                        RoleID = (int)RoleEnum.DistrictAdmin,
                        XLIAreaGroupModuleID = xliAreaGroupModule.XLIAreaGroupModuleID
                    });
                }

                if (request.SchoolAdmin)
                {
                    gmRoles.Add(new XLIAreaGMRoleDto
                    {
                        RoleID = (int)RoleEnum.SchoolAdmin,
                        XLIAreaGroupModuleID = xliAreaGroupModule.XLIAreaGroupModuleID
                    });
                }

                if (request.Teacher)
                {
                    gmRoles.Add(new XLIAreaGMRoleDto
                    {
                        RoleID = (int)RoleEnum.Teacher,
                        XLIAreaGroupModuleID = xliAreaGroupModule.XLIAreaGroupModuleID
                    });
                }

                if (gmRoles.Count > 0)
                    _xliAreaGMRoleRepo.Add(gmRoles.ToArray());
            }
        }

        public void RemoveModulePermission(RemoveModulePermissionRequest request)
        {
            var success = _xliAreaGMRoleRepo.Delete(request.XLIGroupId, request.XLIModuleId);
            if (success)
            {
                _xliAreaGroupModuleRepo.Delete(request.XLIGroupId, request.XLIModuleId);
            }
        }

        public AddModulePermissionRequest GetAreaModuleName(int areaId, int moduleId)
        {
            var area = _repositoryXliArea.Select().FirstOrDefault(x => x.XliAreaId == areaId);
            var module = _repositoryXliModule.Select().FirstOrDefault(x => x.XliModuleId == moduleId);

            return new AddModulePermissionRequest
            {
                XLIAreaId = areaId,
                XLIModuleId = moduleId,
                DisplayHeader = $"{ area?.DisplayTooltip } > { module?.DisplayName }",
                IsNotSupportSchoolAdminAndTeacher = module?.Code == ContaintUtil.DadManageUserGroup
            };
        }

        public IEnumerable<SchoolAccess> GetSchoolAccessData(int moduleId, int districtID)
        {
            var result = _repositoryModuleAccess.GetSchoolAccessByModuleID(moduleId, districtID);

            return result;
        }

        public GetGroupUserResponse GetAllUsersInGroup(GetGroupUserRequest request)
        {
            var response = _userManageRepository.GetUserManageByRoleInGroup(request);
            response.Data = response.Data.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
            return response;
        }

        private IEnumerable<int> GetXliModuleIDs(GetModuleAccessPaginationRequest request)
        {
            var areas = _repositoryIconAccess
                .GetAllXliAreasByUserId(request.UserID, request.DistrictID.Value, request.RoleID)
                .Where(x => AreaCodeSupport.Contains(x.Code));

            var xliAreaIds = string.Join(",", areas.Select(o => o.XliAreaId));
            var modules = _repositoryModuleAccess
                .GetAllModulesAccessByUser(request.UserID, request.DistrictID.Value, request.RoleID, xliAreaIds)
                .Where(x => ModuleCodeSupport.Contains(x.Code))
                .ToList();

            CheckDenyFlashModule(modules);

            if (request.XLIAreaID.HasValue && request.XLIAreaID.Value > 0)
            {
                modules = modules.Where(x => x.XliAreaId == request.XLIAreaID.Value).ToList();
            }

            if (request.XLIModuleID.HasValue && request.XLIModuleID.Value > 0)
            {
                modules = modules.Where(x => x.XliModuleId == request.XLIModuleID.Value).ToList();
            }

            return modules.Select(x => x.XliModuleId);
        }

        private void CheckDenyFlashModule(List<XliModule> xliModules)
        {
            var denyFlashModule = _repositoryConfiguration.Select().FirstOrDefault(o => o.Name.Equals("DenyFlashModule"));
            if (denyFlashModule != null && denyFlashModule.Value.Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                var listFlashModule = _repositoryConfiguration.Select().FirstOrDefault(o => o.Name.Equals("ListFlashModuleCode"));
                if (listFlashModule != null && !string.IsNullOrEmpty(listFlashModule.Value))
                {
                    var listFlashModuleCode = listFlashModule.Value.Split(Convert.ToChar(","));
                    xliModules.RemoveAll(o => listFlashModuleCode.Contains(o.Code));
                }
            }
        }
    }
}
