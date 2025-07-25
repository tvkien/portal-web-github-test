using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using CsvHelper;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Helpers.SGO;
using LinkIt.BubbleSheetPortal.Web.Models.SGO;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.SGO;
using Newtonsoft.Json;
using RestSharp.Extensions;
using ListItem = LinkIt.BubbleSheetPortal.Models.ListItem;
using User = LinkIt.BubbleSheetPortal.Models.User;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class SGOManageController : BaseController
    {
        private readonly SGOManageControllerParameters _parameters;
        private readonly Services.ConfigurationService _configurationService;
        private SGOHomeFilterViewModel SgoFilter
        {
            get
            {
                return (SGOHomeFilterViewModel)Session["SGOHomeFilterViewModel"];
            }
            set
            {
                Session["SGOHomeFilterViewModel"] = value;
            }
        }

        public SGOManageController(SGOManageControllerParameters parameters, Services.ConfigurationService configurationService)
        {
            _parameters = parameters;
            _configurationService = configurationService;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportItemSGOManager)]
        public ActionResult Index()
        {
            var model = new SGOHomeViewModel()
            {
                IsPublisher = CurrentUser.IsPublisher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                CurrentDistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                CurrentSchoolId = CurrentUser.SchoolId.GetValueOrDefault(),
                ListDistricIds = CurrentUser.IsNetworkAdmin ? CurrentUser.GetMemberListDistrictId() : null,
                IsDistrictAdmin = CurrentUser.IsDistrictAdmin,
                IsSchoolAdmin = CurrentUser.IsSchoolAdmin,
                //HasFinalAdministrativeSignoffSGO = _parameters.SGOObjectService.CheckUserHasFinalAdministrativeSignoffSGO(CurrentUser.Id),
                HasFinalAdministrativeSignoffSGO = true
            };

            if (CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher)
            {
                model.CurrentSchoolId = _parameters.UserSchoolServices.GetListSchoolIdByUserId(CurrentUser.Id).FirstOrDefault();
            }

            var vDistrictDecode = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentUser.DistrictId.GetValueOrDefault(), Constanst.SGOHomeDirection);
            if (vDistrictDecode.Any())
            {
                model.Directions = vDistrictDecode.First().Value;
            }

            if (SgoFilter == null)
            {
                SgoFilter = new SGOHomeFilterViewModel
                {
                    IsArchivedStatusActive = true,
                    SGOStatusIds = "1,2,3,4,5,6,7,8,9"
                };

                if (CurrentUser.IsDistrictAdmin || CurrentUser.IsSchoolAdmin)
                {
                    SgoFilter.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
                    SgoFilter.ReviewerId = CurrentUser.Id;
                    SgoFilter.SGOStatusIds = "2,3,4,5,6,7,8,9"; // Default do not load Draft SGO for District Admin and School Admin
                }

                if (CurrentUser.IsSchoolAdmin)
                {
                    SgoFilter.ReviewerId = CurrentUser.Id;
                }

                if (CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher)
                {
                    SgoFilter.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
                }

                if (DateTime.Now >= new DateTime(DateTime.Now.Year, 8, 1))
                {
                    SgoFilter.InstructionPeriodFrom = new DateTime(DateTime.Now.Year, 8, 1);
                }
                else
                {
                    SgoFilter.InstructionPeriodFrom = new DateTime(DateTime.Now.Year - 1, 8, 1);
                }
                SgoFilter.InstructionPeriodTo = SgoFilter.InstructionPeriodFrom.Value.AddYears(1).AddDays(-1);
            }

            model.SgoFilter = SgoFilter;
            model.IsSignedOffSGO = TempData["IsSignedOffSGO"] != null;
            return View(model);
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportItemSGOManager)]
        public ActionResult StudentPopulation(int id)
        {
            ViewBag.SGOID = id;
            var objModel = new StudentPopulationViewModel()
            {
                IsNetworkAdmin = CurrentUser.RoleId == (int)Permissions.NetworkAdmin,
                IsPublisher = CurrentUser.RoleId == (int)Permissions.Publisher,
                CurrentDistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                ListDistricIds = CurrentUser.IsNetworkAdmin ? CurrentUser.GetMemberListDistrictId() : null,
                SGOID = id,
                OwnerUserID = CurrentUser.Id,
                LimitDisplayRoleID = CurrentUser.RoleId,
                CurrentUserID = CurrentUser.Id
            };
            var objSgo = _parameters.SGOObjectService.GetSGOByID(id);
            if (objSgo == null)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            var vPermissionAccess = _parameters.SGOObjectService.GetPermissionAccessSgoStudentPopulate(CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status == (int)SGOPermissionEnum.NotAvalible)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            objModel.PermissionAccess = vPermissionAccess.Status;
            var vOwneruser = _parameters.UserService.GetUserById(objSgo.OwnerUserID);
            if (vOwneruser != null)
            {
                if (vOwneruser.IsNetworkAdmin() || vOwneruser.IsPublisher())
                    objModel.LimitDisplayRoleID = (int)Permissions.DistrictAdmin;
                else
                {
                    objModel.LimitDisplayRoleID = vOwneruser.RoleId;
                }
            }
            if (objSgo.OwnerUserID != CurrentUser.Id)
            {
                //Set Model by owner district
                objModel.IsNetworkAdmin = false;
                objModel.IsPublisher = false;
                objModel.CurrentDistrictId = objSgo.DistrictID;
                objModel.ListDistricIds = null;
                objModel.OwnerUserID = objSgo.OwnerUserID;
            }

            if (id > 0)
            {
                EditSGO(objModel);
            }
            else
            {
                AddNewSGO(objModel);
            }

            int districtId = objModel.CurrentDistrictId > 0 ? objModel.CurrentDistrictId : CurrentUser.DistrictId.GetValueOrDefault();
            var vDistrictDecode = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Constanst.SGOStudentPopulateInstrodution);
            if (vDistrictDecode.Any())
            {
                objModel.SGOIntruction = vDistrictDecode.First().Value;
            }
            return View(objModel);
        }

        [NonAction]
        private void EditSGO(StudentPopulationViewModel obj)
        {
            if (obj.SGOID > 0) //Edit SGO
            {
                var lstStudentFilter = _parameters.SGOStudentFilterService.GetListSGOStudentFilterBySGOID(obj.SGOID);
                if (lstStudentFilter.Any())
                {
                    obj.GenderIdsSeleted =
                        lstStudentFilter.Where(o => o.FilterType == (int)SGOStudentFilterType.Gender)
                            .Select(o => o.FilterID)
                            .ToList();

                    obj.RaceIdsSelected = lstStudentFilter.Where(o => o.FilterType == (int)SGOStudentFilterType.Race)
                            .Select(o => o.FilterID)
                            .ToList();
                    obj.ProgramIdsSelected = lstStudentFilter.Where(o => o.FilterType == (int)SGOStudentFilterType.Program)
                            .Select(o => o.FilterID)
                            .ToList();

                    var vState = lstStudentFilter.FirstOrDefault(o => o.FilterType == (int)SGOStudentFilterType.State);
                    obj.StateIdSelected = vState == null ? 0 : vState.FilterID;

                    var vDistrict = lstStudentFilter.FirstOrDefault(o => o.FilterType == (int)SGOStudentFilterType.District);
                    obj.DistrictIdSelected = vDistrict == null ? 0 : vDistrict.FilterID;
                    obj.CurrentDistrictId = obj.DistrictIdSelected;

                    obj.ListTermIdsSelected =
                        lstStudentFilter.Where(o => o.FilterType == (int)SGOStudentFilterType.DistrictTerm)
                            .Select(o => o.FilterID)
                            .ToList();

                    obj.ListClassIdsSelected = lstStudentFilter.Where(o => o.FilterType == (int)SGOStudentFilterType.Class)
                            .Select(o => o.FilterID)
                            .ToList();
                }
                obj.ListStudentIdsSelected =
                    _parameters.SgoStudentService.GetListStudentBySGOID(obj.SGOID).Select(o => o.StudentID).ToList();
                InitFilter(obj);
            }
        }

        [NonAction]
        private void AddNewSGO(StudentPopulationViewModel obj)
        {
            if (!obj.IsNetworkAdmin && !obj.IsPublisher)
            {
                InitFilter(obj);
            }
        }

        private void InitFilter(StudentPopulationViewModel obj)
        {
            var objFilter = new SGOPopulateStudentFilter()
            {
                SGOId = obj.SGOID,
                DistrictId = obj.CurrentDistrictId,
                UserId = CurrentUser.Id,
                UserRoleId = CurrentUser.RoleId
            };

            if (obj.OwnerUserID != CurrentUser.Id)
            {
                objFilter.UserId = obj.OwnerUserID;
                if (CurrentUser.IsPublisher() || CurrentUser.IsNetworkAdmin)
                {
                    objFilter.UserRoleId = (int)Permissions.DistrictAdmin;
                }
                else
                {
                    objFilter.UserRoleId = obj.LimitDisplayRoleID;
                }
            }
            obj.ListProgram =
                _parameters.SGOObjectService.SGOGetProgram(objFilter)
                    .Select(o => new ListItemsViewModel() { Id = o.Id, Name = o.Name }).OrderBy(o => o.Name).ToList();
            obj.ListProgram.Insert(0, new ListItemsViewModel()
            {
                Id = -1,
                Name = "All Programs"
            });

            obj.ListRace =
                _parameters.SGOObjectService.SGOGetRace(objFilter)
                    .Select(o => new ListItemsViewModel() { Id = o.Id, Name = o.Name }).OrderBy(o => o.Name).ToList();
            obj.ListGender =
                _parameters.SGOObjectService.SGOGetGender(objFilter)
                    .Select(o => new ListItemsViewModel() { Id = o.Id, Name = o.Name }).OrderBy(o => o.Name).ToList();

        }

        public ActionResult GetGenderByDistrictID(int sgoId, int districtId, int userId, int userRoleId)
        {
            if (districtId > 0 && sgoId > 0)
            {
                var obj = new SGOPopulateStudentFilter()
                {
                    DistrictId = districtId,
                    UserId = userId, //CurrentUser.Id,
                    UserRoleId = userRoleId, //CurrentUser.RoleId,
                    SGOId = sgoId
                };

                var result =
                    _parameters.SGOObjectService.SGOGetGender(obj)
                        .Select(o => new ListItemsViewModel() { Id = o.Id, Name = Server.HtmlEncode(o.Name) })
                        .OrderBy(o => o.Name)
                        .ToList();
                var lstGenderIds = _parameters.SGOStudentFilterService.GetGenderIdsBySGOId(obj.SGOId);
                return Json(new { Sucess = true, data = result, lstIds = lstGenderIds }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { sucess = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRaceByDistrictID(int sgoId, int districtId, int userId, int userRoleId)
        {
            if (districtId > 0)
            {
                var obj = new SGOPopulateStudentFilter()
                {
                    DistrictId = districtId,
                    UserId = userId,
                    UserRoleId = userRoleId,
                    SGOId = sgoId
                };
                var result =
                    _parameters.SGOObjectService.SGOGetRace(obj)
                        .Select(o => new ListItemsViewModel() { Id = o.Id, Name = Server.HtmlEncode(o.Name) })
                        .OrderBy(o => o.Name)
                        .ToList();
                var lstRaceIds = _parameters.SGOStudentFilterService.GetRaceIdsBySGOId(obj.SGOId);
                return Json(new { Sucess = true, data = result, lstIds = lstRaceIds }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { sucess = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProgramByDistrictID(int sgoId, int districtId, int userId, int userRoleId)
        {
            if (districtId > 0)
            {
                var obj = new SGOPopulateStudentFilter()
                {
                    DistrictId = districtId,
                    UserId = userId,
                    UserRoleId = userRoleId,
                    SGOId = sgoId
                };
                var result =
                    _parameters.SGOObjectService.SGOGetProgram(obj)
                        .Select(o => new ListItemsViewModel() { Id = o.Id, Name = Server.HtmlEncode(o.Name) })
                        .OrderBy(o => o.Name)
                        .ToList();
                result.Insert(0, new ListItemsViewModel()
                {
                    Id = -1,
                    Name = "All programs"
                });
                var lstProgramIds = _parameters.SGOStudentFilterService.GetProgramIdsBySGOId(obj.SGOId);
                return Json(new { Sucess = true, data = result, lstIds = lstProgramIds }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { sucess = false }, JsonRequestBehavior.AllowGet);
        }

        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult GetTerms(SGOPopulateStudentFilter obj)
        {
            var districtTerms = _parameters.SGOObjectService.SGOGetDistictTerm(obj)
             .Select(x => new ListItem
             {
                 Id = x.Id,
                 Name = x.Name
             }).ToList();
            return Json(districtTerms, JsonRequestBehavior.AllowGet);
        }

        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult GetClassesByTermId(SGOPopulateStudentFilter obj)
        {
            var data = _parameters.SGOObjectService.SGOGetClasses(obj)
                .Select(o => new ListItemsViewModel() { Id = o.Id, Name = o.Name });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStudentsbyClassIds(SGOPopulateStudentFilter obj)
        {
            obj.UserId = CurrentUser.Id;
            obj.UserRoleId = CurrentUser.RoleId;
            var studentsInClass = _parameters.SGOObjectService.SGOGetStudents(obj).ToList();

            if (obj.IncludeAddedStudents.GetValueOrDefault())
            {
                var unmanagedStudentOfSgo = GetUnmanagedStudentOfSgo(obj);

                // Append current students in SGO to make sure that selected students are displayed on screen
                studentsInClass.AddRange(unmanagedStudentOfSgo.Where(x => studentsInClass.All(k => k.Id != x.Id)));
            }

            var data = studentsInClass
                .Select(x => new { x.Id, x.Name, x.ExtracId })
                .OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public List<ListItemExtra> GetUnmanagedStudentOfSgo(SGOPopulateStudentFilter obj)
        {
            var studentsInSgo = _parameters.SGOObjectService.GetStudentSelectedBySogId(obj.SGOId);
            var unmanagedStudentsOfSgo = new List<ListItemExtra>();

            var studentInSgoClassIds = studentsInSgo.Select(x => x.ExtracId);
            if (studentInSgoClassIds.Any())
            {
                var sgoObject = _parameters.SGOObjectService.GetSGOByID(obj.SGOId);
                var sgoOwner = _parameters.UserService.GetUserById(sgoObject.OwnerUserID);

                foreach (var classId in studentInSgoClassIds)
                {
                    if (!_parameters.VulnerabilityService.CheckUserCanAccessClass(sgoOwner, classId))
                    {
                        unmanagedStudentsOfSgo.AddRange(studentsInSgo.Where(x => x.ExtracId == classId));
                    }
                }
            }

            return unmanagedStudentsOfSgo;
        }

        public ActionResult GetSGOInstance(SGOHomeFilterViewModel model)
        {
            SgoFilter = model; // Store filter information into session

            var parser = new DataTableParserProc<SGOInstanceViewModel>();
            int? totalRecords = 0;
            var sortColumns = parser.SortableColumns;
            var searchKey = parser.SearchInBox.Any() ? parser.SearchInBox[0] : string.Empty;

            var lst = _parameters.SGOObjectService.GetSGOCustom(model.DistrictId, CurrentUser.Id, parser.StartIndex, parser.PageSize, ref totalRecords
                , sortColumns, searchKey, CurrentUser.RoleId
                , model.IsArchivedStatusArchived, model.IsArchivedStatusActive, model.SchoolId, model.TeacherId
                , model.ReviewerId, model.DistrictTermId, model.SGOStatusIds
                , model.InstructionPeriodFrom, model.InstructionPeriodTo)
                .ToList()
                .Select(o => new SGOInstanceViewModel()
                {
                    ID = o.ID,
                    Name = o.Name,
                    Teacher = o.Teacher,
                    School = o.School,
                    GradeIDs = o.Grade,
                    Course = o.Course,
                    TotalStudent = o.TotalStudent.GetValueOrDefault(),
                    StartDate = o.StartDate.HasValue ? string.Format("{0} {1}", o.StartDate.GetValueOrDefault().DisplayDateWithFormat(), o.EndDate.GetValueOrDefault().DisplayDateWithFormat()) : string.Empty,
                    CreatedDate = o.CreatedDate.DisplayDateWithFormat(),
                    EffectiveStatus = o.EffectiveStatus,
                    EffectiveStatusDate = o.EffectiveStatusDate.HasValue ? o.EffectiveStatusDate.Value.DisplayDateWithFormat() : string.Empty,
                    Version = o.Version,
                    IsArchived = o.IsArchived,
                    IsOwner = o.OwnerUserID == CurrentUser.Id,
                    //NotEdit = o.OwnerUserID != CurrentUser.Id && o.ApproverUserID != CurrentUser.Id,
                    NotEdit = false, // Users can edit all sgo accessed by them
                    ApproverName = o.ApproverName
                }).AsQueryable();
            return Json(parser.Parse(lst, totalRecords ?? 0), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadCreateSgo(int? id)
        {
            var sgoModel = new SGOObjectViewModel()
            {
                ListFullGrades = _parameters.GradeService.GetGrades().OrderBy(o => o.Order)
                .Select(o => new ListItemsViewModel()
                {
                    Id = o.Id,
                    Name = o.Name
                }).ToList()
            };
            sgoModel.PermissionAccess = 3; //Update
            if (id.HasValue && id.Value > 0)
            {
                var vSGO = _parameters.SGOObjectService.GetSGOByID(id.GetValueOrDefault());
                if (vSGO != null)
                {
                    sgoModel.SGOId = vSGO.SGOID;
                    sgoModel.StrName = vSGO.Name;
                    sgoModel.StrStartDate = vSGO.StartDate.HasValue
                        ? vSGO.StartDate.Value.DisplayDateWithFormat()
                        : string.Empty;
                    sgoModel.StrEndDate = vSGO.EndDate.HasValue ? vSGO.EndDate.Value.DisplayDateWithFormat() : string.Empty;
                    sgoModel.ListGradeSelected = Util.ParseListStringtoListInt(vSGO.GradeIDs, ',');
                    sgoModel.PermissionAccess = _parameters.SGOObjectService.GetPermissionAccessSgoHome(CurrentUser.Id, vSGO).Status;
                    sgoModel.IsUnstructuredSgo = vSGO.Type == 2;
                    sgoModel.AssosiatedSchoolId = vSGO.Type == 2 && !string.IsNullOrEmpty(vSGO.SchoolIDs) ? Convert.ToInt32(vSGO.SchoolIDs) : (int?)null;
                }
            }
            sgoModel.DefaultWeek = _parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue(Constanst.SGODefaultWeek, 9);

            return PartialView("_CreateSGO", sgoModel);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateSGOByName(SGOObjectViewModel objViewModel)
        {
            var vName = objViewModel.StrName.Trim();
            bool isCreate = true;
            if (!string.IsNullOrEmpty(vName))
            {
                var obj = new SGOObject();
                if (objViewModel.SGOId == 0)
                {
                    obj.CreatedDate = DateTime.UtcNow;
                    obj.SGOStatusID = (int)SGOStatusType.Draft;
                    obj.IsArchive = false;
                    obj.OwnerUserID = CurrentUser.Id;
                    obj.Version = 1; //Init alway 1
                    obj.TargetScoreType = 0;
                    obj.DistrictID = CurrentUser.DistrictId.GetValueOrDefault();
                    obj.Type = objViewModel.IsUnstructuredSgo ? 2 : 1;
                    if (objViewModel.IsUnstructuredSgo)
                    {
                        obj.SchoolIDs = objViewModel.AssosiatedSchoolId.HasValue ? objViewModel.AssosiatedSchoolId.GetValueOrDefault().ToString() : null;
                    }
                }
                else
                {
                    obj = _parameters.SGOObjectService.GetSGOByID(objViewModel.SGOId);
                    if (obj == null)
                    {
                        return Json(new { Success = false, sgoId = 0, ErrorMessage = "There's no SGO" }, JsonRequestBehavior.AllowGet);
                    }
                    isCreate = false;

                    var vPermissionAccess = _parameters.SGOObjectService.GetPermissionAccessSgoHome(CurrentUser.Id, obj);
                    if (vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate)
                    {
                        return Json(new { Success = false, sgoId = 0, ErrorMessage = "Has no permission" }, JsonRequestBehavior.AllowGet);
                    }
                }
                obj.SGOID = objViewModel.SGOId;
                obj.Name = vName;
                DateTime? startDate, endDate;
                endDate = startDate = null;
                DateTime dt = DateTime.UtcNow;

                if (objViewModel.StrStartDate.TryParseDateWithFormat(out dt))
                {
                    startDate = dt;
                }
                obj.EndDate = DateTime.UtcNow;

                if (objViewModel.StrEndDate.TryParseDateWithFormat(out dt))
                {
                    endDate = dt;
                }
                obj.GradeIDs = objViewModel.ListGradeIds;
                //check ajax parameters modify
                if (string.IsNullOrWhiteSpace(obj.GradeIDs))
                {

                    return Json(new { Success = false, sgoId = 0, ErrorMessage = "You must first select a " + LabelHelper.GradeLabel + "(s)" }, JsonRequestBehavior.AllowGet);
                }

                if (obj.Type == (int)SGOTypeEnum.UnstructuredData)
                {
                    obj.SchoolIDs = objViewModel.AssosiatedSchoolId.HasValue ? objViewModel.AssosiatedSchoolId.GetValueOrDefault().ToString() : null;
                }

                //check grade if it's valid or not
                var validGradeIdist = _parameters.GradeService.GetGrades().Select(x => x.Id).ToList();
                var submittedGradeIdList = obj.GradeIDs.ParseIdsFromString();
                foreach (var gradeId in submittedGradeIdList)
                {
                    if (!validGradeIdist.Contains(gradeId))
                    {
                        return Json(new { Success = false, sgoId = 0, ErrorMessage = "One or more grade is invalid" }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (string.IsNullOrWhiteSpace(obj.Name))
                {
                    return Json(new { Success = false, sgoId = 0, ErrorMessage = "Name is required" }, JsonRequestBehavior.AllowGet);
                }
                if (!startDate.HasValue)
                {
                    return Json(new { Success = false, sgoId = 0, ErrorMessage = "Invalid start date" }, JsonRequestBehavior.AllowGet);
                }
                if (!endDate.HasValue)
                {
                    return Json(new { Success = false, sgoId = 0, ErrorMessage = "Invalid end date" }, JsonRequestBehavior.AllowGet);
                }
                if (startDate.Value > endDate.Value)
                {
                    return Json(new { Success = false, sgoId = 0, ErrorMessage = "Start Date cannot be greater than End Date" }, JsonRequestBehavior.AllowGet);
                }
                obj.StartDate = startDate.Value;
                obj.EndDate = endDate.Value;
                obj.UpdatedDate = DateTime.UtcNow;
                _parameters.SGOObjectService.Save(obj);
                if (isCreate)
                    _parameters.SGOMilestoneService.CreateMilestone(obj.SGOID, CurrentUser.Id);

                //Init default group if no group is existed
                var groups = _parameters.SgoGroupService.GetGroupBySgoID(obj.SGOID);
                if (!groups.Any())
                {
                    //Initial default group for SGO
                    _parameters.SgoGroupService.InitialDefaultGroupForSGO(obj.SGOID);
                }

                return Json(new { Success = true, sgoId = obj.SGOID }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, sgoId = 0 }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportItemSGOManager)]
        public ActionResult EstablishStudentGroups(int id)
        {
            ViewBag.SGOID = id;
            var objSgo = _parameters.SGOObjectService.GetSGOByID(id);
            if (objSgo == null)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            var vPermissionAccess = _parameters.SGOObjectService.GetPermissionAccessSgoPreparednessGroup(CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status == (int)SGOPermissionEnum.NotAvalible)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            ViewBag.PermissionAccess = vPermissionAccess.Status;

            ViewBag.PreparednessGroupDirections = string.Empty;
            var vDistrictDecode = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(objSgo.DistrictID, Constanst.SGOPreparednessGroupDirection);
            if (vDistrictDecode.Any())
            {
                ViewBag.PreparednessGroupDirections = vDistrictDecode.First().Value;
            }
            return View(id);
        }

        [SGOManagerLogFilter]
        public ActionResult ShowStudentGroupTable(int sgoId, bool? isAutoGroup)
        {
            var model = BuildEstablishStudentGroupViewModel(sgoId, isAutoGroup);

            return PartialView("_StudentGroupTable", model);
        }

        private EstablishStudentGroupViewModel BuildEstablishStudentGroupViewModel(int sgoId, bool? isAutoGroup)
        {
            var objSgo = _parameters.SGOObjectService.GetSGOByID(sgoId);
            var vPermissionAccess =
                _parameters.SGOObjectService.GetPermissionAccessSgoPreparednessGroup(CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate)
            {
                isAutoGroup = false;
            }

            //Get list groups of this SGO
            var groups = _parameters.SgoGroupService.GetGroupBySgoID(sgoId).ToList();
            if (!groups.Any())
            {
                //Initial default group for SGO
                _parameters.SgoGroupService.InitialDefaultGroupForSGO(sgoId);
                groups = _parameters.SgoGroupService.GetGroupBySgoID(sgoId)
                    .OrderBy(x => x.Order).ToList();
            }
            _parameters.SgoGroupService.MoveStudentHasNoGroupToDefaultGroup(sgoId);

            //Get list data points of this SGO
            var dataPoints = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId)
                .Where(x => x.Type != (int)SGODataPointTypeEnum.PostAssessment
                            && x.Type != (int)SGODataPointTypeEnum.PostAssessmentCustom
                            && x.Type != (int)SGODataPointTypeEnum.PostAssessmentToBeCreated
                            && x.Type != (int)SGODataPointTypeEnum.PostAssessmentExternal
                            && x.Type != (int)SGODataPointTypeEnum.PostAssessmentHistorical).ToList();

            //init default bands for data points
            foreach (var sgoDataPoint in dataPoints)
            {
                if (!isAutoGroup.GetValueOrDefault())
                {
                    //re-populate data for datapoint, insert testResultID to SGOStudentDataPoint table
                    if (sgoDataPoint.VirtualTestID.HasValue)
                        _parameters.SgoSelectDataPointService.SaveStudentDataPointFromVirtualTest(sgoId,
                            sgoDataPoint.SGODataPointID, sgoDataPoint.VirtualTestID.GetValueOrDefault());
                    _parameters.SgoDataPointService.InitDefaultBandForDataPoint(sgoDataPoint.SGODataPointID); //chua co custom
                }
                sgoDataPoint.IsCustomCutScore = _parameters.SgoDataPointService.CheckIsCustomCutScore(sgoDataPoint); //chua co custom
            }

            //Get list students of this SGO
            var sgoStudents = _parameters.SgoStudentService.GetListStudentBySGOID(sgoId);
            var students = _parameters.SGOObjectService
                .GetAllStudentsBySogId(sgoId).Select(x => new ListItemsViewModel { Name = x.Name, Id = x.Id })
                .ToList();


            var dcScoreTypes = new Dictionary<int, List<ListItemStr>>();
            //Get list student data point
            var listStudentScoreInDataPoint = new Dictionary<int, List<SGOStudentScoreInDataPointViewModel>>();
            foreach (var sgoDataPoint in dataPoints)
            {
                var scoreType = SGOHelper.GetDataPointScoreType(sgoDataPoint);
                var scoreInDataPoint = GetStudentScoreInDataPoint(sgoDataPoint, scoreType, 0, true);
                var dataPointBand =
                    _parameters.SgoDataPointService.GetDataPointBandByDataPointID(sgoDataPoint.SGODataPointID).OrderBy(x=>x.LowValue).ToList();
                AssignDataPointBandColor(scoreInDataPoint, dataPointBand, sgoDataPoint);
                if (!isAutoGroup.GetValueOrDefault())
                {
                    SaveStudentDataPointBand(scoreInDataPoint, dataPointBand);
                }
                listStudentScoreInDataPoint.Add(sgoDataPoint.SGODataPointID, scoreInDataPoint);
                if (!dcScoreTypes.ContainsKey(sgoDataPoint.SGODataPointID))
                {
                    dcScoreTypes.Add(sgoDataPoint.SGODataPointID, GetCustomScoreType(sgoDataPoint, objSgo.DistrictID));
                }

                //get subscoreId for preAssessmentCustom
                if (sgoDataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom)
                {
                    var clusterScore = _parameters.SgoDataPointClusterScoreService.GetDataPointClusterScoreBySGODataPointID(sgoDataPoint.SGODataPointID).FirstOrDefault();
                    if (clusterScore != null)
                    {
                        sgoDataPoint.VirtualTestCustomScoreId = clusterScore.VirtualTestCustomSubScoreId;
                    }
                }
            }

            if (isAutoGroup.GetValueOrDefault())
            {
                var autoGroupData = _parameters.SgoGroupService.AutoGroup(sgoId, false, 0);
                foreach (var sgoStudent in sgoStudents)
                {
                    var studentGroup = autoGroupData.FirstOrDefault(x => x.SGOStudentID == sgoStudent.SGOStudentID);
                    if (studentGroup != null)
                    {
                        sgoStudent.SGOGroupID = studentGroup.SGOGroupID;
                    }
                }
            }

            var model = new EstablishStudentGroupViewModel();
            model.ListStudents = students;
            model.ListDataPoint = dataPoints;
            model.ListGroups = groups.OrderBy(o => o.Order).ToList();
            model.ListSGOStudents = sgoStudents;
            model.SGOID = sgoId;
            model.ListStudentScoreInDataPoint = listStudentScoreInDataPoint;
            model.ExistCustomDataPoint =
                model.ListDataPoint.Any(
                    o =>
                        o.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom ||
                        o.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom);
            model.DicScoreTypes = dcScoreTypes;
            return model;
        }

        [HttpGet]
        public ActionResult GetDataPointForStudent(int sgoId, bool? isAutoGroup)
        {
            var objSgo = _parameters.SGOObjectService.GetSGOByID(sgoId);
            var vPermissionAccess =
                _parameters.SGOObjectService.GetPermissionAccessSgoPreparednessGroup(CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate)
            {
                isAutoGroup = false;
            }

            //Get list data points of this SGO
            var dataPoints = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId)
                .Where(x => x.Type != (int)SGODataPointTypeEnum.PostAssessment
                            && x.Type != (int)SGODataPointTypeEnum.PostAssessmentCustom
                            && x.Type != (int)SGODataPointTypeEnum.PostAssessmentToBeCreated
                            && x.Type != (int)SGODataPointTypeEnum.PostAssessmentExternal
                            && x.Type != (int)SGODataPointTypeEnum.PostAssessmentHistorical).ToList();

            //init default bands for data points
            foreach (var sgoDataPoint in dataPoints)
            {
                if (!isAutoGroup.GetValueOrDefault())
                {
                    //re-populate data for datapoint, insert testResultID to SGOStudentDataPoint table
                    if (sgoDataPoint.VirtualTestID.HasValue)
                        _parameters.SgoSelectDataPointService.SaveStudentDataPointFromVirtualTest(sgoId,
                            sgoDataPoint.SGODataPointID, sgoDataPoint.VirtualTestID.GetValueOrDefault());
                    _parameters.SgoDataPointService.InitDefaultBandForDataPoint(sgoDataPoint.SGODataPointID);
                }
                sgoDataPoint.IsCustomCutScore = _parameters.SgoDataPointService.CheckIsCustomCutScore(sgoDataPoint);
            }

            var dcScoreTypes = new Dictionary<int, List<ListItemStr>>();
            //Get list student data point
            var listStudentScoreInDataPoint = new List<SGOStudentScoreViewModel>();
            foreach (var sgoDataPoint in dataPoints)
            {
                var scoreType = SGOHelper.GetDataPointScoreType(sgoDataPoint);
                var scoreInDataPoint = GetStudentScoreInDataPoint(sgoDataPoint, scoreType);
                var dataPointBand =
                    _parameters.SgoDataPointService.GetDataPointBandByDataPointID(sgoDataPoint.SGODataPointID).ToList();
                AssignDataPointBandColor(scoreInDataPoint, dataPointBand, sgoDataPoint);
                if (!isAutoGroup.GetValueOrDefault())
                {
                    SaveStudentDataPointBand(scoreInDataPoint, dataPointBand);
                }
                if (!dcScoreTypes.ContainsKey(sgoDataPoint.SGODataPointID))
                {
                    dcScoreTypes.Add(sgoDataPoint.SGODataPointID, GetCustomScoreType(sgoDataPoint, objSgo.DistrictID));
                }

                var model = new SGOStudentScoreViewModel()
                {
                    SGODataPointID = sgoDataPoint.SGODataPointID,
                    Type = sgoDataPoint.Type,
                    StudentScoreInDataPoints = scoreInDataPoint
                };
                listStudentScoreInDataPoint.Add(model);
            }
            var groupId = 0;
            var sgoTobePlaceGroup = _parameters.SgoGroupService.GetGroupBySgoID(sgoId).FirstOrDefault(x => x.Order == Constanst.ToBePlacedGroupOrder);
            if (sgoTobePlaceGroup != null)
                groupId = sgoTobePlaceGroup.SGOGroupID;
            return Json(new { dataPoints = listStudentScoreInDataPoint, groupId = groupId }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidateDefaultPointBandForAutoGroup(int sgoId)
        {
            // Check whether all historical datapoints have default point bands to show warning message before auto-group
            var isSuccess = true;
            int errorType = -1;
            var sgoDataPoints = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId);
            var dataPointName = string.Empty;
            foreach (var sgoDataPoint in sgoDataPoints)
            {
                if (sgoDataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical)
                {
                    dataPointName = sgoDataPoint.Name;
                    var sgoDataPointBands =
                        _parameters.SgoDataPointService.GetDataPointBandByDataPointID(sgoDataPoint.SGODataPointID);
                    if (!sgoDataPointBands.Any())
                    {
                        isSuccess = false;
                        errorType = 1;
                        break;
                    }
                    // Check whether historical datapoints with default cut score and filter more than 2 cluster scores
                    if (!_parameters.SgoDataPointService.CheckIsCustomCutScore(sgoDataPoint) && _parameters.SgoDataPointService.GetDataPointClusterFilterCount(sgoDataPoint.SGODataPointID) > 1)
                    {
                        isSuccess = false;
                        errorType = 2;
                        break;
                    }
                }
            }

            return Json(new { success = isSuccess, errorType, dataPointName }, JsonRequestBehavior.AllowGet);
        }

        private List<SGOStudentScoreInDataPointViewModel> GetStudentScoreInDataPoint(SGODataPoint sgoDataPoint, int scoreType, int? virtualTestCustomSubScoreId = 0, bool? fromLoading = false)
        {
            var tempSgoDataPoint = _parameters.SgoDataPointService.GetById(sgoDataPoint.SGODataPointID);
            var tempScoreType = tempSgoDataPoint.ScoreType;
            tempSgoDataPoint.ScoreType = scoreType;
            _parameters.SgoDataPointService.Save(tempSgoDataPoint);
            SGOScoreTypeEnum scoreTypeEnum = (SGOScoreTypeEnum)scoreType;

            var scoreInDataPoint = _parameters.SgoDataPointService
                .GetScoreInDataPoint(sgoDataPoint.SGODataPointID, virtualTestCustomSubScoreId)
                .Select(x => 
                {
                    var item = new SGOStudentScoreInDataPointViewModel();
                    item.StudentID = x.StudentID;
                    item.TotalScore = x.TotalPointPossible > 0 ? x.TotalPointPossible : sgoDataPoint.TotalPoints;

                    switch (scoreTypeEnum)
                    {
                        case SGOScoreTypeEnum.ScoreRaw: item.Score = x.ScoreRaw; break;
                        case SGOScoreTypeEnum.ScoreScaled: item.Score = x.ScoreScaled; break;
                        case SGOScoreTypeEnum.ScorePercentage: item.Score = x.ScorePercentage; break;
                        case SGOScoreTypeEnum.ScoreIndex: item.Score = x.ScoreIndex; break;
                        case SGOScoreTypeEnum.ScoreLexile: item.Score = x.ScoreLexile; break;
                        case SGOScoreTypeEnum.ScoreCustomN1: item.Score = x.ScoreCustomN_1; break;
                        case SGOScoreTypeEnum.ScoreCustomN2: item.Score = x.ScoreCustomN_2; break;
                        case SGOScoreTypeEnum.ScoreCustomN3: item.Score = x.ScoreCustomN_3; break;
                        case SGOScoreTypeEnum.ScoreCustomN4: item.Score = x.ScoreCustomN_4; break;
                        default: item.Score = x.ScorePercent; break;
                    }

                    item.ScorePercent = x.ScorePercent;
                    item.TotalQuestions = x.TotalQuestion;
                    item.ScoreText = x.ScoreText;
                    item.AchievementLevel = x.AchievementLevel.GetValueOrDefault(-1);

                    return item;
                })
                .ToList();

            tempSgoDataPoint.ScoreType = tempScoreType;
            _parameters.SgoDataPointService.Save(tempSgoDataPoint);

            //Case PreAssessmentCustom
            if (sgoDataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom)
            {
                var labelValueDic =  new Dictionary<decimal, string>();
                var metaData = new VirtualTestCustomMetaModel();
                var isCustomLabelValue = false;
                //get metadata to check label/value type
                if (fromLoading.GetValueOrDefault() && virtualTestCustomSubScoreId.GetValueOrDefault() == 0)
                {
                    var clusterScore = _parameters.SgoDataPointClusterScoreService.GetDataPointClusterScoreBySGODataPointID(sgoDataPoint.SGODataPointID).FirstOrDefault();
                    if (clusterScore != null)
                        virtualTestCustomSubScoreId = clusterScore.VirtualTestCustomSubScoreId;
                }

                if (virtualTestCustomSubScoreId.GetValueOrDefault() > 0)
                {
                    var scoreTypeName = Enum.GetName(typeof(DataLockerScoreTypeEnum), scoreType);
                    var virtualTestCustomSubMetaData = _parameters.DataLockerService.GetVirtualTestCustomMetaDataByVirtualTestCustomSubScoreID(virtualTestCustomSubScoreId.GetValueOrDefault()).FirstOrDefault(x=> x.ScoreType == scoreTypeName);
                    if (virtualTestCustomSubMetaData != null)
                        metaData = JsonConvert.DeserializeObject<VirtualTestCustomMetaModel>(virtualTestCustomSubMetaData.MetaData);
                }
                else if (tempSgoDataPoint.AchievementLevelSettingID.GetValueOrDefault() > 0)
                {
                    var scoreTypeName = Enum.GetName(typeof(DataLockerScoreTypeEnum), scoreType);
                    var virtualTestCustomMetaData =
                        _parameters.DataLockerService.GetVirtualTestCustomMetaDataByVirtualTestCustomScoreId(
                            tempSgoDataPoint.AchievementLevelSettingID.GetValueOrDefault()).FirstOrDefault(x=>!x.VirtualTestCustomSubScoreID.HasValue && x.ScoreType == scoreTypeName);
                    if (virtualTestCustomMetaData != null)
                        metaData = JsonConvert.DeserializeObject<VirtualTestCustomMetaModel>(virtualTestCustomMetaData.MetaData);
                }

                if (metaData != null && metaData.FormatOption != null && metaData.FormatOption.ToLower() == "labelvaluetext")
                {
                    foreach (var selectOption in metaData.SelectListOptions)
                    {
                        decimal key;
                        if (decimal.TryParse(selectOption.Option, out key))
                        {
                            if (metaData.DisplayOption == "label")
                            {
                                isCustomLabelValue = true;
                                labelValueDic.Add(key, selectOption.Label);
                            }
                            else if (metaData.DisplayOption == "both")
                            {
                                isCustomLabelValue = true;
                                labelValueDic.Add(key, string.Format("{0} ({1})", selectOption.Label, selectOption.Option));
                            }
                            else
                            {
                                labelValueDic.Add(key, selectOption.Option);
                            }
                        }
                    }
                }
                
                foreach (var studentscore in scoreInDataPoint)
                {
                    decimal dScore = -1;
                    studentscore.Score = dScore;
                    if (!string.IsNullOrEmpty(studentscore.ScoreText) && decimal.TryParse(studentscore.ScoreText, out dScore))
                        studentscore.Score = dScore;

                    if (metaData != null && metaData.FormatOption != null && metaData.FormatOption.ToLower() == "labelvaluetext" && isCustomLabelValue)
                    {
                        string scoreText;
                        labelValueDic.TryGetValue(studentscore.Score.GetValueOrDefault(), out scoreText);
                        studentscore.ScoreText = scoreText;
                        studentscore.IsCustomLabelValue = true;
                    }
                }
            }
            return scoreInDataPoint;
        }
        private void SaveStudentDataPointBand(List<SGOStudentScoreInDataPointViewModel> scoreInDataPoint, List<SGODataPointBand> dataPointBand)
        {
            foreach (var sgoDataPointBand in dataPointBand)
            {
                var studentIdList = scoreInDataPoint.Where(x => x.DataPointBandID == sgoDataPointBand.SGODataPointBandID)
                    .Select(x => x.StudentID).ToList();
                if (studentIdList.Any())
                {
                    var studentIDsString = string.Join(",", studentIdList);
                    _parameters.SgoDataPointService.AssignStudentToDataPointBand(sgoDataPointBand.SGODataPointBandID, studentIDsString);
                }
            }
        }
        private void AssignDataPointBandColor(List<SGOStudentScoreInDataPointViewModel> scoreInDataPoint, List<SGODataPointBand> dataPointBand, SGODataPoint dataPoint)
        {
            if (!dataPointBand.Any()) return;
            var listColor = ColorHelper.GetColorHexList(dataPointBand.Count);
            var clusterFilterCount =
                _parameters.SgoDataPointService.GetDataPointClusterFilterCount(dataPoint.SGODataPointID);
            var isHistoricalDataPointWithDefaultBand = dataPoint.Type ==
                                                       (int)SGODataPointTypeEnum.PreAssessmentHistorical && !dataPoint.IsCustomCutScore;
            var useAchievementLevelForColor = isHistoricalDataPointWithDefaultBand && clusterFilterCount <= 1;

            if (useAchievementLevelForColor)
            {
                //Remove this code, because dataPointBand always order low to high
                GetAchievementLevelForDataPointBand(dataPointBand, dataPoint);
            }
            foreach (var viewModel in scoreInDataPoint)
            {
                double percent = -1;
                int achievementLevel = -1;
                if (dataPoint.Type == (int)SGODataPointTypeEnum.PreAssessment || dataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentExternal)
                {
                    if (viewModel.TotalScore == 0) continue;
                    percent = (double)(viewModel.Score.GetValueOrDefault() / viewModel.TotalScore) * 100;
                }
                else if (dataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical || dataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom)
                {
                    percent = (double)viewModel.Score.GetValueOrDefault();
                    if (!dataPoint.IsCustomCutScore)
                    {
                        achievementLevel = viewModel.AchievementLevel;
                    }
                }
                else
                {
                    continue;
                }
                SGODataPointBand band = null;
                if (useAchievementLevelForColor)
                {
                    band = dataPointBand.FirstOrDefault(x => x.AchievementLevel == achievementLevel);
                }
                else if (!isHistoricalDataPointWithDefaultBand)
                {
                    band = dataPointBand.FirstOrDefault(x => x.LowValue <= percent && percent <= x.HighValue);
                }

                if (band == null) continue;

                viewModel.DataPointBandID = band.SGODataPointBandID;
                var index = dataPointBand.IndexOf(band);
                var colorHex = string.Empty;

                if (index >= 0 && index < listColor.Count) colorHex = listColor[index];
                viewModel.Color = colorHex;
            }
        }

        private void GetAchievementLevelForDataPointBand(List<SGODataPointBand> dataPointBand, SGODataPoint dataPoint)
        {
            var defaultDataPointBand = _parameters.SgoDataPointService.CreateDefaultBandForLegacyTest(dataPoint);
            foreach (var band in dataPointBand)
            {
                var defaultBand =
                    defaultDataPointBand.FirstOrDefault(
                        x => x.Name == band.Name && x.HighValue == band.HighValue && x.LowValue == band.LowValue);
                if (defaultBand != null)
                {
                    band.AchievementLevel = defaultBand.AchievementLevel;
                }
            }
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [AjaxOnly]
        public ActionResult SaveStudentPopulate(StudentPopulationViewModel obj)
        {
            //Check permission
            var objSgo = _parameters.SGOObjectService.GetSGOByID(obj.SGOID);
            if (objSgo == null) return Json(false);
            var vPermissionAccess = _parameters.SGOObjectService.GetPermissionAccessSgoStudentPopulate(CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate) return Json(false, JsonRequestBehavior.AllowGet);
            //End Check permission
            if (obj.DistrictIdSelected > 0 && !Util.HasRightOnDistrict(CurrentUser, obj.DistrictIdSelected))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }


            // parse classStudent to xml & call save 
            var lst = new List<SGOStudentData>();
            if (!string.IsNullOrEmpty(obj.StrStudentIds))
            {
                string[] arr = obj.StrStudentIds.Split(',');
                if (arr.Length > 0)
                {
                    foreach (var s in arr)
                    {
                        string[] item = s.Split('-');
                        if (item.Count() == 2)
                        {
                            int studentId = 0;
                            int.TryParse(item[0], out studentId);
                            int classId = 0;
                            int.TryParse(item[1], out classId);
                            if (studentId > 0 && classId > 0)
                            {
                                lst.Add(new SGOStudentData()
                                {
                                    StudentId = studentId,
                                    ClassId = classId
                                });
                            }
                        }
                    }
                }
            }

            var currentSgoStudents = _parameters.SgoStudentService.GetListStudentBySGOID(obj.SGOID);

            var newAddedStudent = lst.Where(x => !currentSgoStudents.Any(k => k.StudentID == x.StudentId));
            // Do not verify vulnerability in case new student list is sub-set of current student list in database to handle the case when teachers do not manage list student already added before
            // If CurrentSGOStudent list in database does not cover new student list
            if (!_parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser,
                string.Join(",", newAddedStudent.Select(x => x.StudentId))))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            //check access class
            if (!_parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser,
                newAddedStudent.Select(x => x.ClassId).Distinct().ToList()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            //Save & init DataPoint
            var objXml = new ETLXmlSerialization<List<SGOStudentData>>();
            var vResult = objXml.SerializeObjectToXmlWithOutHeader(lst);
            _parameters.SGOObjectService.SaveClassStudent(obj.SGOID, vResult);
            //Save Gender, Grade & Program, Term & Class Selected
            var vObjFilter = new SGOPopulateStudentFilter()
            {
                SGOId = obj.SGOID,
                GenderIds = obj.StrGenderIds,
                RaceIds = obj.StrRaceIds,
                ProgramIds = obj.StrProgramIds,
                TermIds = obj.StrDistrictTermIds,
                ClassIds = obj.StrClassIds
            };
            _parameters.SGOObjectService.SaveStudentPopulate(vObjFilter);
            _parameters.SGOObjectService.UpdateDistrictSelected(obj.SGOID, obj.DistrictIdSelected);
            _parameters.SGOStudentFilterService.SaveDistrictIdAndStateIdBySGOId(obj.SGOID, obj.StateIdSelected, obj.DistrictIdSelected);
            _parameters.SGOObjectService.PopulateSchoolIdsAndClassIdsBySgoId(obj.SGOID);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadStudentBySGOID(int sgoId)
        {
            var data = _parameters.SGOObjectService.GetStudentSelectedBySogId(sgoId).AsQueryable();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadStudentIntervalIntruction(int districtId)
        {
            var vDistrictId = districtId > 0 ? districtId : CurrentUser.DistrictId.GetValueOrDefault();
            ViewBag.StudentIntervalStudent = string.Empty;
            var isUseNewDesign = HelperExtensions.IsUseNewDesign(vDistrictId);
            var labelSGOStudentsInterval = isUseNewDesign ? Constanst.SGOStudentsInterval + "_NewSkin" : Constanst.SGOStudentsInterval;
            var vDistrictDecode = _parameters.DistrictDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(vDistrictId, labelSGOStudentsInterval);
            if (vDistrictDecode != null)
            {
                ViewBag.StudentIntervalStudent = vDistrictDecode.Value;
            }

            return PartialView("_StudentIntervalIntruction");
        }

        public ActionResult LoadCreateGroup(int sgoId)
        {
            var obj = new SGOAddEditGroupViewModel();
            obj.LstGroups = _parameters.SgoGroupService.GetGroupWithOut9899BySgoID(sgoId)
                .Select(o => new ListItemsViewModel()
                {
                    Id = o.Order,
                    Name = o.Name
                }).ToList();

            return PartialView("_CreateGroups", obj);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        //[AjaxOnly]
        public ActionResult SaveGroupBySOGId(SGOUpdateGroupViewModel model)
        {
            var objSgo = _parameters.SGOObjectService.GetSGOByID(model.SGOID);
            var vPermissionAccess = _parameters.SGOObjectService.GetPermissionAccessSgoPreparednessGroup(CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                //Log your exception
                return Json(new { ErrorMessage = "Has no permission" }, JsonRequestBehavior.AllowGet);
            }

            var lstSGOGroup = new List<ListItem>();
            if (!string.IsNullOrEmpty(model.StrGroups))
            {
                string[] lstGroup = model.StrGroups.Split(new string[] { "-|-" }, StringSplitOptions.None);
                if (lstGroup.Any())
                {
                    foreach (var item in lstGroup)
                    {
                        string[] lstGroupItem = item.Split(new string[] { "|-|" }, StringSplitOptions.None);
                        if (lstGroupItem.Length == 2)
                        {
                            int groupOrder = 0;
                            if (int.TryParse(lstGroupItem[0], out groupOrder) && groupOrder > 0 && !string.IsNullOrEmpty(lstGroupItem[1]))
                            {
                                lstSGOGroup.Add(new ListItem()
                                {
                                    Id = groupOrder,
                                    Name = lstGroupItem[1]
                                });
                            }
                        }
                    }
                    _parameters.SgoGroupService.SaveSGOGroup(model.SGOID, lstSGOGroup);
                }
            }
            else
            {
                //DELETE ALL GROUPS
                _parameters.SgoGroupService.SaveSGOGroup(model.SGOID, lstSGOGroup);
            }

            var establishStudentGroupViewModel = BuildEstablishStudentGroupViewModel(model.SGOID, true);
            foreach (var sgoStudent in establishStudentGroupViewModel.ListSGOStudents)
            {
                var groupInfo = model.StudentInGroups.FirstOrDefault(x => x.StudentId == sgoStudent.StudentID);
                if (groupInfo != null &&
                    establishStudentGroupViewModel.ListGroups.Any(x => x.SGOGroupID == groupInfo.GroupId))
                {
                    sgoStudent.SGOGroupID = groupInfo.GroupId;
                }
                else
                {
                    sgoStudent.SGOGroupID =
                        establishStudentGroupViewModel.ListGroups.First(x => x.Order == Constanst.ToBePlacedGroupOrder)
                            .SGOGroupID;
                }
            }

            return PartialView("_StudentGroupTable", establishStudentGroupViewModel);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [AjaxOnly]
        public ActionResult SaveStudentGroup(SGOStudentGroupUploadViewModel data)
        {
            if (data == null) return Json(false, JsonRequestBehavior.AllowGet);
            if (data.StudentInGroups == null) return Json(false, JsonRequestBehavior.AllowGet);

            var objSgo = _parameters.SGOObjectService.GetSGOByID(data.SGOID);


            if (objSgo == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (objSgo.SGOStatusID == (int)SGOStatusType.Draft && objSgo.OwnerUserID != CurrentUser.Id)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (objSgo.SGOStatusID == (int)SGOStatusType.PreparationSubmittedForApproval && objSgo.ApproverUserID != CurrentUser.Id && objSgo.OwnerUserID != CurrentUser.Id)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var vPermissionAccess = _parameters.SGOObjectService.GetPermissionAccessSgoPreparednessGroup(CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status == (int)SGOPermissionEnum.MinorUpdate && !ValidateSaveStudentGroupForMinorUpdate(data.SGOID, data.StudentInGroups.ToList()))
            {
                return Json(false);
            }
            if (vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate
                && vPermissionAccess.Status != (int)SGOPermissionEnum.MinorUpdate)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (vPermissionAccess.Status == (int)SGOPermissionEnum.FullUpdate && data.DataPointScoreType != null && data.DataPointScoreType.Any())
            {
                var preCustomAssessmentScoreType = 0;
                foreach (var dataPointScoreType in data.DataPointScoreType.ToList())
                {
                    var dataPoint = _parameters.SgoDataPointService.GetById(dataPointScoreType.DataPointId);
                    if (dataPoint != null && dataPointScoreType.ScoreType > 0)
                    {
                        if (dataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom)
                        {
                            preCustomAssessmentScoreType = dataPointScoreType.ScoreType;

                            var clusterScore =
                                    _parameters.SgoDataPointClusterScoreService.GetDataPointClusterScoreBySGODataPointID
                                        (dataPointScoreType.DataPointId).FirstOrDefault();
                            if (dataPointScoreType.VirtualTestCustomSubScoreId.HasValue && dataPointScoreType.VirtualTestCustomSubScoreId > 0)
                            {
                                if (clusterScore != null)
                                {
                                    clusterScore.VirtualTestCustomSubScoreId = dataPointScoreType.VirtualTestCustomSubScoreId;
                                }
                                else
                                {
                                    clusterScore = new SGODataPointClusterScore
                                    {
                                        VirtualTestCustomSubScoreId = dataPointScoreType.VirtualTestCustomSubScoreId,
                                        TestResultSubScoreName = string.Empty,
                                        SGODataPointID = dataPointScoreType.DataPointId
                                    };
                                }
                                _parameters.SgoDataPointClusterScoreService.Save(clusterScore);
                            }
                            else if (clusterScore != null)
                            {
                                _parameters.SgoDataPointClusterScoreService.Delete(clusterScore);
                            }
                        }

                        dataPoint.ScoreType = dataPointScoreType.ScoreType;
                        _parameters.SgoDataPointService.Save(dataPoint);
                    }
                }
                //Update ScoreType PostCustomAssessment.
                //var vDataPointPostAssessmentCustom =
                //    _parameters.SgoDataPointService.GetDataPointBySGOID(data.SGOID)
                //        .FirstOrDefault(o => o.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom);
                //if (preCustomAssessmentScoreType > 0
                //    && vDataPointPostAssessmentCustom != null
                //    && vDataPointPostAssessmentCustom.ScoreType != preCustomAssessmentScoreType)
                //{
                //    vDataPointPostAssessmentCustom.ScoreType = preCustomAssessmentScoreType;
                //    _parameters.SgoDataPointService.Save(vDataPointPostAssessmentCustom);
                //}
            }

            var listGroupIDs = data.StudentInGroups.Select(x => x.GroupId).Distinct().ToList();
            foreach (var groupId in listGroupIDs)
            {
                var sgoGroup = _parameters.SgoGroupService.GetGroupById(groupId);

                // Allow to move student to Excluded group when status = minor-update
                if (vPermissionAccess.Status == (int)SGOPermissionEnum.FullUpdate
                    || vPermissionAccess.Status == (int)SGOPermissionEnum.MinorUpdate
                    || sgoGroup.Order == 99)
                {
                    var listStudentIDs =
                        data.StudentInGroups.Where(x => x.GroupId == groupId).Select(x => x.StudentId).ToList();
                    if (listStudentIDs.Any())
                    {
                        var studentIdString = string.Join(",", listStudentIDs);
                        _parameters.SgoGroupService.AssignStudentToGroup(data.SGOID, groupId, studentIdString);
                    }
                }
            }

            _parameters.SGOObjectService.PopulateSchoolIdsAndClassIdsBySgoId(data.SGOID);
            //check if there is no group, return false
            var groupCount = _parameters.SgoGroupService.GetGroupWithOut9899BySgoID(data.SGOID).Count();
            return Json(groupCount > 0, JsonRequestBehavior.AllowGet);
        }

        private bool ValidateSaveStudentGroupForMinorUpdate(int sgoId,
            List<UploadStudentInGroupViewModel> studentInGroups)
        {
            // Just allow to move assigned students to excluded group (not allow to move between groups or move to To Be Placed group)

            var result = true;

            var sgoGroupIds =
                _parameters.SgoGroupService.GetGroupWithOut9899BySgoID(sgoId)
                    .Select(x => x.SGOGroupID).ToList();
            var sgoStudents = _parameters.SgoStudentService.GetListStudentBySGOID(sgoId).ToList();

            // Get list force unchanged group students
            var unchangedSgoStudents =
                sgoStudents.Where(x => sgoGroupIds.Contains(x.SGOGroupID.GetValueOrDefault())).ToList();

            var sgoExcludedGroupId = 0;
            var sgoExcludedGroup = _parameters.SgoGroupService.GetGroupBySgoID(sgoId).FirstOrDefault(x => x.Order == 99);
            if (sgoExcludedGroup != null)
            {
                sgoExcludedGroupId = sgoExcludedGroup.SGOGroupID;
            }

            foreach (var item in studentInGroups)
            {
                if (
                    unchangedSgoStudents.Any(
                        x => x.StudentID == item.StudentId && x.SGOGroupID != item.GroupId && item.GroupId != sgoExcludedGroupId))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AutoGroup(int sgoID, bool includeUpdate)
        {
            var autoGroupData = _parameters.SgoGroupService.AutoGroup(sgoID, includeUpdate, 0);
            return Json(new { data = autoGroupData }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTermsSelected(int sgoId)
        {
            var districtTerms = _parameters.SGOStudentFilterService.GetListTermSelectedBySGOID(sgoId)
             .Select(x => new { Id = x.FilterID }).ToList();
            return Json(districtTerms, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetClassSelected(int sgoId)
        {
            var classIds = _parameters.SGOStudentFilterService.GetListClassSelectedBySGOID(sgoId)
             .Select(x => new { Id = x.FilterID }).ToList();
            return Json(classIds, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCandidateForReplacementRemovedClass(int sgoId)
        {


            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadDataPointBandByDataPointId(int datapointId)
        {
            var obj = new SGOAddEditBandViewModel() { DataPointId = datapointId };
            var dataPoint = _parameters.SgoDataPointService.GetById(datapointId);

            if (dataPoint != null)
            {
                if (dataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical
                    && _parameters.SgoDataPointService.CheckIsCustomCutScore(dataPoint))
                {
                    obj.LstDataPointBands = _parameters.SgoDataPointService.GetDataPointBandByDataPointID(datapointId)
                        .OrderByDescending(o => o.AchievementLevel)
                        .Select(o => new DataPointBandViewModel()
                        {
                            Id = o.SGODataPointBandID,
                            DataPointId = o.SGODataPointID,
                            High = o.HighValue,
                            Low = o.LowValue,
                            Name = o.Name
                        }).ToList();
                }
                else if (dataPoint.Type != (int)SGODataPointTypeEnum.PreAssessmentHistorical)
                {
                    obj.LstDataPointBands =
                        _parameters.SgoDataPointService.GetDataPointBandByDataPointID(datapointId)
                            .OrderBy(o => o.LowValue)
                            .Select(o => new DataPointBandViewModel()
                            {
                                Id = o.SGODataPointBandID,
                                DataPointId = o.SGODataPointID,
                                High = o.HighValue,
                                Low = o.LowValue,
                                Name = o.Name
                            }).ToList();
                }
                else
                {
                    obj.LstDataPointBands =
                        _parameters.SgoDataPointService.GetDataPointBandByDataPointID(datapointId)
                            .OrderBy(x => x.SGODataPointBandID) // Order by Id to keep the order in AchievementLevelSetting.LabelString when saving to database
                            .Select(o => new DataPointBandViewModel()
                            {
                                Id = o.SGODataPointBandID,
                                DataPointId = o.SGODataPointID,
                                High = o.HighValue,
                                Low = o.LowValue,
                                Name = o.Name
                            }).ToList();
                }
            }

            return PartialView("_AddEditBand", obj);
        }

        public ActionResult LoadDataPointWeightBysgoId(int sgoId)
        {
            var obj = new DataPointWeighViewModel();
            var query = _parameters.SgoDataPointService.GetPreAssessmentDataPointBySGOID(sgoId).ToList();
            double total = query.Sum(o => o.Weight);
            total = total > 0 ? total : 1;
            obj.LstWeights = query.Select(o => new ListItemExtra()
            {
                Id = o.SGODataPointID,
                Name = o.Name,
                ExtracId = (int)o.Weight,
                ExtraField = ((int)((o.Weight / total) * 100)).ToString()
            }).ToList();
            return PartialView("_ChangeWeights", obj);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [AjaxOnly]
        public ActionResult SaveWeightChange(string strDataPoint)
        {
            var isValidateStatus = false;

            string[] arr = strDataPoint.Split(',');
            if (arr.Length > 0)
            {
                List<KeyValuePair<int, int>> listWeights = new List<KeyValuePair<int, int>>();
                foreach (var s in arr)
                {
                    string[] arrDataPointWeight = s.Split('-');
                    if (arrDataPointWeight.Length == 2)
                    {
                        int dataPointId = 0;
                        int weight = 0;
                        int.TryParse(arrDataPointWeight[0], out dataPointId);
                        int.TryParse(arrDataPointWeight[1], out weight);
                        if (dataPointId > 0 && weight >= 0)
                        {
                            if (!isValidateStatus)
                            {
                                var sgoPermissionStatus = GetSgoPermissionStatusByDataPointId(dataPointId);
                                if (sgoPermissionStatus != (int)SGOPermissionEnum.FullUpdate)
                                {
                                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                                }
                                isValidateStatus = true;
                            }

                            _parameters.SgoDataPointService.UpdateWeightById(dataPointId, weight);
                            listWeights.Add(new KeyValuePair<int, int>(dataPointId, weight));
                        }
                    }
                }
                var responseData = listWeights.Select(x => new
                {
                    DataPointId = x.Key,
                    Weight = x.Value.ToString("0"),
                    Percent =
                                                                   string.Format("{0:00}%",
                                                                       (x.Value * 100 / listWeights.Sum(y => y.Value)))
                }).ToList();
                return Json(new { success = true, data = responseData }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        private int GetSgoPermissionStatusByDataPointId(int dataPointId)
        {
            var sgoDataPoint = _parameters.SgoDataPointService.GetById(dataPointId);
            if (sgoDataPoint != null)
            {
                var objSgo = _parameters.SGOObjectService.GetSGOByID(sgoDataPoint.SGOID);
                var vPermissionAccess =
                    _parameters.SGOObjectService.GetPermissionAccessSgoPreparednessGroup(CurrentUser.Id, objSgo);
                return vPermissionAccess.Status;
            }

            return (int)SGOPermissionEnum.NotAvalible;
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [AjaxOnly]
        public ActionResult InitDefaultCutScore(int dataPointId)
        {
            var sgoPermissionStatus = GetSgoPermissionStatusByDataPointId(dataPointId);
            if (sgoPermissionStatus != (int)SGOPermissionEnum.FullUpdate)
            {
                return Json(new { isCustom = false, success = false }, JsonRequestBehavior.AllowGet);
            }

            //delete current data point band
            var datapointBands = _parameters.SgoDataPointService.GetDataPointBandByDataPointID(dataPointId);
            _parameters.SgoDataPointService.DeleteDataPointBand(datapointBands);
            _parameters.SgoDataPointService.InitDefaultBandForDataPoint(dataPointId);
            return Json(new { isCustom = false, success = true }, JsonRequestBehavior.AllowGet);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [AjaxOnly]
        public ActionResult SaveCutScoreByDataPoint(int dataPointId, string strCutScore)
        {
            if (dataPointId > 0 && !string.IsNullOrEmpty(strCutScore))
            {
                var sgoPermissionStatus = GetSgoPermissionStatusByDataPointId(dataPointId);
                if (sgoPermissionStatus != (int)SGOPermissionEnum.FullUpdate)
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }

                string[] arr = strCutScore.Split(new string[] { "-^-" }, StringSplitOptions.None);
                if (arr.Length > 0)
                {
                    var lst = ConvertStringCutScoreToListDataPointBand(dataPointId, arr);
                    var datapointBands = _parameters.SgoDataPointService.GetDataPointBandByDataPointID(dataPointId).ToList();
                    var lstBandIdsWillDelete = new List<SGODataPointBand>();
                    foreach (var sgoDataPointBand in datapointBands)
                    {
                        var v = lst.FirstOrDefault(o => o.SGODataPointBandID == sgoDataPointBand.SGODataPointBandID);
                        if (v != null)
                        {
                            _parameters.SgoDataPointService.UpdateDataPointBand(v);
                            lst.Remove(v);
                        }
                        else
                        {
                            lstBandIdsWillDelete.Add(sgoDataPointBand);
                        }
                    }
                    if (lstBandIdsWillDelete.Count > 0)
                    {
                        _parameters.SgoDataPointService.DeleteDataPointBand(lstBandIdsWillDelete);
                    }
                    if (lst.Count > 0)
                    {
                        _parameters.SgoDataPointService.AddDataPointBand(lst);
                    }
                }

                bool bIsCustom = false;
                var sgoDataPoint = _parameters.SgoDataPointService.GetById(dataPointId);
                if (sgoDataPoint != null)
                    bIsCustom = _parameters.SgoDataPointService.CheckIsCustomCutScore(sgoDataPoint);
                return Json(new { isCustom = bIsCustom, success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        private List<SGODataPointBand> ConvertStringCutScoreToListDataPointBand(int dataPointId, IEnumerable<string> arr)
        {
            var lst = new List<SGODataPointBand>();
            foreach (var s in arr)
            {
                string[] arrBandObj = s.Split('|');
                if (arrBandObj.Length >= 4)
                {
                    int bandId = 0;
                    string strName = arrBandObj[1];
                    double dHigh = 0;
                    double dLow = 0;
                    int.TryParse(arrBandObj[0], out bandId);
                    double.TryParse(arrBandObj[2], out dHigh);
                    double.TryParse(arrBandObj[3], out dLow);
                    if (dHigh >= 0 && dLow >= 0 && dHigh >= dLow)
                    {
                        lst.Add(new SGODataPointBand()
                        {
                            SGODataPointBandID = bandId,
                            SGODataPointID = dataPointId,
                            HighValue = dHigh,
                            LowValue = dLow,
                            Name = strName
                        });
                    }
                }
            }
            return lst;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CheckReadyForSubmit(int id)
        {
            var sgoObject = _parameters.SGOObjectService.GetSGOByID(id);
            if (sgoObject == null)
                return Json(new { success = false });

            if (sgoObject.Type == (int)SGOTypeEnum.UnstructuredData)
            {
                var sgoGroups = _parameters.SgoGroupService.GetGroupWithOut9899BySgoID(id);

                // At least one goal is saved
                if (!sgoGroups.Any())
                {
                    return Json(new { success = false });
                }
            }
            else
            {
                var allStudentInGroup = false;
                var group98 = _parameters.SgoGroupService.GetGroupBySgoID(id).FirstOrDefault(o => o.Order == 98);
                if (group98 != null)
                {
                    allStudentInGroup = _parameters.SgoStudentService.AllStudentInGroup(id, group98.SGOGroupID);
                }

                var hasPostAssessment = _parameters.SgoDataPointService.HasPostAssessment(id);
                if (!allStudentInGroup || !hasPostAssessment)
                {
                    return Json(new { success = false, studentInGroup = allStudentInGroup, postAssessment = hasPostAssessment }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SubmitForReview(int? id)
        {
            var obj = _parameters.SGOObjectService.GetSGOByID(id.GetValueOrDefault());
            if (!id.HasValue || obj == null)
            {
                return PartialView("_SubmitForReview", new SubmitForReviewGetModel());
            }

            var districtAdmins =
                _parameters.SGOObjectService.SGOGetAdminsOfUser(obj.DistrictID, CurrentUser.Id, CurrentUser.RoleId)
                    .Select(o => new UserModel
                    {
                        UserID = o.Id,
                        LastName = o.LastName,
                        FirstName = o.FirstName
                    }).ToList();

            var model = new SubmitForReviewGetModel
            {
                SGOID = id.Value,
                DistrictAdmins = districtAdmins,
            };

            return PartialView("_SubmitForReview", model);
        }

        public ActionResult GetReviewers(int districtId)
        {
            var districtAdmins = _parameters.SGOObjectService.SGOGetAdminsOfUser(districtId, CurrentUser.Id, CurrentUser.RoleId).ToList();

            // Add current user because SGOGetAdminsOfUser function excluded current user out of the list
            if (CurrentUser.IsDistrictAdmin || CurrentUser.IsSchoolAdmin)
            {
                var dbUser = _parameters.UserService.Select().FirstOrDefault(x => x.Id == CurrentUser.Id);
                if (dbUser != null)
                {
                    districtAdmins.Add(new User
                    {
                        Id = dbUser.Id,
                        LastName = dbUser.LastName,
                        FirstName = dbUser.FirstName
                    });
                }
            }

            var vResult = districtAdmins.OrderBy(x => x.LastName).Select(x => new
            {
                Id = x.Id,
                Name = x.LastName + ", " + x.FirstName
            }).ToList();
            return Json(vResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAssociatedSchools()
        {
            var data = _parameters.UserSchoolServices.GetSchoolsUserHasAccessTo(CurrentUser.Id)
                    .Select(x => new ListItem { Id = x.SchoolId.GetValueOrDefault(), Name = x.SchoolName })
                    .OrderBy(x => x.Name);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTeachers(int schoolId)
        {
            var validUserSchoolRoleId = new[] { 2, 3, 5, 8, 27 };
            if (!CurrentUser.IsPublisher && !_parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(new { error = "Has no right to the selected school." }, JsonRequestBehavior.AllowGet);
            }

            //var data = schoolTeacherService.GetTeachersBySchoolId(schoolId).Select(x => new
            var data = _parameters.UserSchoolServices.GetSchoolsUserBySchoolId(schoolId).Where(x => validUserSchoolRoleId.Contains((int)x.Role));
            if (CurrentUser.RoleId == (int)Permissions.Teacher)
            {
                data = data.Where(o => o.UserId == CurrentUser.Id);
            }

            var vResult = data.OrderBy(x => x.LastName).Select(x => new
            {
                Id = x.UserId,
                Name = x.LastName + ", " + x.FirstName
            }).ToList();

            return Json(vResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDistrictTerms(int districtId)
        {
            var districtTerms = _parameters.DistrictTermService.GetAllTermsByDistrictID(districtId);

            var data = districtTerms.OrderBy(x => x.Name).Select(x => new ListItem
            {
                Id = x.DistrictTermID,
                Name = x.Name
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [AjaxOnly]
        public ActionResult SubmitForReviewPost(SubmitForReviewPostModel model)
        {
            if (model == null || !model.SGOID.HasValue || !model.DistrictAdminID.HasValue)
            {
                return Json(new { Status = "error" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var sgo = _parameters.SGOObjectService.GetSGOByID(model.SGOID.GetValueOrDefault());
                if (sgo == null)
                {
                    return Json(new { Status = "fail", Error = "SGO does not exist." }, JsonRequestBehavior.AllowGet);
                }
                if (sgo.OwnerUserID != CurrentUser.Id)//Tung little advice
                {
                    return Json(new { Status = "fail", Error = "Have no permission." }, JsonRequestBehavior.AllowGet);
                }
                //check reviewer
                var approver = _parameters.SGOObjectService.SGOGetAdminsOfUser(sgo.DistrictID, CurrentUser.Id, CurrentUser.RoleId);
                if (approver.All(x => x.Id != model.DistrictAdminID))
                {
                    return Json(new { Status = "fail", Error = "Approver is invalid." }, JsonRequestBehavior.AllowGet);
                }
                _parameters.SGOMilestoneService.CreateMilestoneWithStatus(model.SGOID.Value, CurrentUser.Id, (int)SGOStatusType.PreparationSubmittedForApproval);
                _parameters.SGOObjectService.ChangeSGOStatus(model.SGOID.Value, model.DistrictAdminID.Value, (int)SGOStatusType.PreparationSubmittedForApproval);
                //\Finish Save Reviewer.
                SendMailSubmitReviewer(model.DistrictAdminID.GetValueOrDefault(), model.SGOID.GetValueOrDefault());
                return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Status = "fail", Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [AjaxOnly]
        public ActionResult ArchiveSgo(int sgoId)
        {
            //set Archive SGO
            var obj = _parameters.SGOObjectService.GetSGOByID(sgoId);
            if (obj != null)
            {
                obj.IsArchive = !obj.IsArchive;
                _parameters.SGOObjectService.Save(obj);
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }

        private string SendMailSubmitReviewer(int reviewerUserId, int sgoId)
        {
            var objReviewer = _parameters.UserService.GetUserById(reviewerUserId);
            var objSgo = _parameters.SGOObjectService.GetSGOByID(sgoId);
            if (objReviewer == null || string.IsNullOrEmpty(objReviewer.EmailAddress))
            {
                return "User or Email Invalid.";
            }

            var objEmailTemplate =
                _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(objSgo.DistrictID,
                    Constanst.Configuration_SGOEmailTemplate).FirstOrDefault();
            string strBody = objEmailTemplate == null ? string.Empty : objEmailTemplate.Value;
            strBody = strBody.Replace("<SGOName>", objSgo.Name);
            strBody = strBody.Replace("<TeacherName>", GetUserFullName(objSgo.OwnerUserID));
            string strSubject = "Submit Reviewer";
            Util.SendMailSGO(strBody, strSubject, objReviewer.EmailAddress);
            return string.Empty;
        }

        private string GetUserFullName(int userId)
        {
            var fullName = string.Empty;
            var user = _parameters.UserService.GetUserById(userId);
            if (user != null)
            {
                fullName = string.Format("{0}, {1}", user.LastName, user.FirstName);
            }

            return fullName;
        }

        public ActionResult ListStepBySGOId(int sgoid)
        {
            var lst = _parameters.SGOObjectService.GetListStepBySGOId(sgoid).OrderBy(o => o.Step);

            var sgoType = 0;
            var sgoObject = _parameters.SGOObjectService.GetSGOByID(sgoid);
            if (sgoObject != null)
            {
                sgoType = sgoObject.Type;
            }
            return Json(new { success = true, data = lst, sgoType = sgoType }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadNavigation(int sgoId)
        {
            ViewBag.NavigationSGOID = sgoId;
            var sgoObject = _parameters.SGOObjectService.GetSGOByID(sgoId);
            if (sgoObject != null)
            {
                var user = _parameters.UserService.GetUserById(sgoObject.OwnerUserID);
                if (user != null)
                {
                    ViewBag.SGOName = sgoObject.Name;
                    ViewBag.OwnerLastName = user.LastName;
                }
            }
            return PartialView("_NavigationTabs");
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportItemSGOManager)]
        public ActionResult AdminReview(int id)
        {
            var objSgo = _parameters.SGOObjectService.GetSGOByID(id);
            if (objSgo == null)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            var vPermissionAccess = _parameters.SGOObjectService.GetPermissionAccessSgoAdminReview(CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status == (int)SGOPermissionEnum.NotAvalible)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            var model = new SGOObjectViewModel()
            {
                SGOId = objSgo.SGOID,
                PermissionAccess = vPermissionAccess.Status,
                IsReviewer = objSgo.ApproverUserID.GetValueOrDefault() == CurrentUser.Id
            };
            if (!string.IsNullOrEmpty(objSgo.Feedback))
            {
                model.Feedback = objSgo.Feedback;
            }
            var vDistrictDecode = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(objSgo.DistrictID, Constanst.SGOAdminReviewDirection);
            if (vDistrictDecode.Any())
            {
                model.AdminReviewDirections = vDistrictDecode.First().Value;
            }
            return View(model);
        }

        public ActionResult GetScoreForHistoricalTest(int dataPointId, int scoreType, int? virtualTestCustomSubScoreId)
        {
            var sgoDataPoint = _parameters.SgoDataPointService.GetById(dataPointId);
            if (scoreType == -1)
            {
                scoreType = SGOHelper.GetDataPointScoreType(sgoDataPoint);
            }
            var scoreInDataPoint = GetStudentScoreInDataPoint(sgoDataPoint, scoreType, virtualTestCustomSubScoreId);
            var dataPointBand =
                _parameters.SgoDataPointService.GetDataPointBandByDataPointID(dataPointId)
                    .OrderBy(x => x.LowValue)
                    .ToList();
            AssignDataPointBandColor(scoreInDataPoint, dataPointBand, sgoDataPoint);
            return Json(new { data = scoreInDataPoint, DataPointType = sgoDataPoint.Type }, JsonRequestBehavior.AllowGet);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult SGOApprove(int sgoId, string strFeedback)
        {
            var obj = _parameters.SGOObjectService.GetSGOByID(sgoId);
            if (obj != null)
            {
                var vPermissionAccess = _parameters.SGOObjectService.GetPermissionAccessSgoAdminReview(CurrentUser.Id, obj);
                if (vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

                _parameters.SGOMilestoneService.CreateMilestoneWithStatus(obj.SGOID, CurrentUser.Id, (int)SGOStatusType.PreparationApproved);
                obj.Feedback = strFeedback;
                obj.SGOStatusID = (int)SGOStatusType.PreparationApproved;
                obj.PreparationApprovedDate = DateTime.UtcNow;
                _parameters.SGOObjectService.Save(obj);
                SendMailApproveOrDeny(obj);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult SGODeny(int sgoId, string strFeedback)
        {
            if (string.IsNullOrWhiteSpace(strFeedback))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            //check permission
            var obj = _parameters.SGOObjectService.GetSGOByID(sgoId);
            if (obj == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            if (obj.SGOStatusID != (int)SGOStatusType.PreparationSubmittedForApproval)
            {
                //Not deny SGO without status Prep Submitted for Approval
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            if (obj.ApproverUserID != CurrentUser.Id)
            {
                //Only approver is authorized to deny
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            obj.SGOStatusID = (int)SGOStatusType.PreparationDenied;
            obj.Feedback = strFeedback.Trim();
            _parameters.SGOObjectService.Save(obj);
            CloneSGOAndUpdateVersion(sgoId, (int)SGOStatusType.PreparationDenied);
            SendMailApproveOrDeny(obj);
            return Json(true, JsonRequestBehavior.AllowGet);

        }

        private void SendMailApproveOrDeny(SGOObject obj)
        {
            string strBody = string.Empty;
            string strSubject = string.Empty;
            var objEmailTemplate = new DistrictDecode();
            if (obj.SGOStatusID == (int)SGOStatusType.PreparationApproved)
            {
                strSubject = "Preparation Approved";
                objEmailTemplate = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(obj.DistrictID, Constanst.Configuration_SGOEmailTemplatePreApprove).FirstOrDefault();
            }
            else if (obj.SGOStatusID == (int)SGOStatusType.PreparationDenied)
            {
                strSubject = "Preparation Denied";
                objEmailTemplate = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(obj.DistrictID, Constanst.Configuration_SGOEmailTemplatePreDeny).FirstOrDefault();
            }
            if (objEmailTemplate != null)
            {
                strBody = objEmailTemplate.Value;
            }
            strBody = strBody.Replace("<SGOName>", obj.Name);
            strBody = strBody.Replace("<Feedback>", WebUtility.HtmlEncode(obj.Feedback).Replace("\n", "<br />"));
            var vUser = _parameters.UserService.GetUserById(obj.OwnerUserID);
            var vReviewUser = _parameters.UserService.GetUserById(obj.ApproverUserID.GetValueOrDefault());
            if (vUser != null && vReviewUser != null)
            {
                strBody = strBody.Replace("<ApprovedUser>", string.Format("{0}, {1}", vReviewUser.LastName, vReviewUser.FirstName));
                Util.SendMailSGO(strBody, strSubject, vUser.EmailAddress);
            }
        }

        public ActionResult ChangeReviewer(int sgoId)
        {
            var obj = _parameters.SGOObjectService.GetSGOByID(sgoId);
            if (obj == null)
            {
                return PartialView("_ChangeReviewer", new SubmitForReviewGetModel());
            }

            var approver = _parameters.SGOObjectService.SGOGetAdminsOfUser(obj.DistrictID, CurrentUser.Id, CurrentUser.RoleId);
            if (obj.ApproverUserID != null)
            {
                approver = approver.Where(o => o.Id != obj.ApproverUserID).ToList();
            }

            var model = new SubmitForReviewGetModel
            {
                SGOID = sgoId,
                DistrictAdmins = approver.Select(o => new UserModel
                {
                    UserID = o.Id,
                    LastName = o.LastName,
                    FirstName = o.FirstName
                }).ToList()
            };

            return PartialView("_ChangeReviewer", model);
        }

        #region Authorize Revision
        [SGOManagerLogFilter]
        [HttpPost]
        [AjaxOnly]
        public ActionResult ChangeReviewerPost(SubmitForReviewPostModel model)
        {
            if (model == null || !model.SGOID.HasValue || !model.DistrictAdminID.HasValue)
            {
                return Json(new { Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var obj = _parameters.SGOObjectService.GetSGOByID(model.SGOID.GetValueOrDefault());

                if (obj == null)
                {
                    return Json(new { Status = "fail", Error = "SGO does not exist." }, JsonRequestBehavior.AllowGet);
                }
                if (!obj.ApproverUserID.HasValue)
                {
                    return Json(new { Status = "fail", Error = "SGO does not be approved." }, JsonRequestBehavior.AllowGet);
                }
                if (obj.OwnerUserID != CurrentUser.Id && obj.ApproverUserID != CurrentUser.Id)//Tung little advice
                {
                    return Json(new { Status = "fail", Error = "No permissions to change reviewer." }, JsonRequestBehavior.AllowGet);
                }

                obj.ApproverUserID = model.DistrictAdminID.GetValueOrDefault();
                _parameters.SGOObjectService.Save(obj);
                //\Finish Save Reviewer.
                SendMailSubmitReviewer(model.DistrictAdminID.GetValueOrDefault(), model.SGOID.GetValueOrDefault());
                return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Status = "fail", Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AuthorizeRevision(int sgoId, string strFeedback)
        {
            var objSgo = _parameters.SGOObjectService.GetSGOByID(sgoId);
            if (objSgo == null)
            {
                return RedirectToAction("Index", "SGOManage");
            }

            var newSgoId = CloneSGOAndUpdateVersion(sgoId, (int)SGOStatusType.Cancelled);

            if (newSgoId > 0)
            {
                var oldSgo = _parameters.SGOObjectService.GetSGOByID(sgoId);
                oldSgo.Feedback = strFeedback.Trim();
                _parameters.SGOObjectService.Save(oldSgo);

                var newSgo = _parameters.SGOObjectService.GetSGOByID(newSgoId);
                newSgo.Feedback = strFeedback.Trim();
                _parameters.SGOObjectService.Save(newSgo);

                SendAuthorizeRevisonEmail(oldSgo);
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        private void SendAuthorizeRevisonEmail(SGOObject obj)
        {
            string strBody = "The <SGOName> has been authorized for revision by <b> <ApprovedUser> </b>.<br /><br /><comments>";
            string strSubject = "Authorize Revision";
            var objEmailTemplate = _parameters.DistrictDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(obj.DistrictID, Constanst.Configuration_SGOEmailTemplateAuthorizeRevision);

            if (objEmailTemplate != null && !string.IsNullOrEmpty(objEmailTemplate.Value))
            {
                strBody = objEmailTemplate.Value;
            }
            strBody = strBody.Replace("<SGOName>", obj.Name);
            var vUser = _parameters.UserService.GetUserById(obj.OwnerUserID);
            var vReviewUser = _parameters.UserService.GetUserById(obj.ApproverUserID.GetValueOrDefault());
            if (vUser != null && vReviewUser != null && !string.IsNullOrEmpty(vUser.EmailAddress))
            {
                var strEmail = vUser.EmailAddress;
                strBody = strBody.Replace("<ApprovedUser>", string.Format("{0}, {1}", vReviewUser.LastName, vReviewUser.FirstName));
                strBody = strBody.Replace("<comments>", WebUtility.HtmlEncode(obj.Feedback).Replace("\n", "<br />"));

                Util.SendMailSGO(strBody, strSubject, strEmail);
            }
        }

        private int CloneSGOAndUpdateVersion(int sgoId, int statusId)
        {
            //Init table Level1 arrount SGO table.
            var newSgoId = 0;
            newSgoId = _parameters.SGOObjectService.SGOAuthorizeRevision(sgoId, CurrentUser.Id, statusId);
            // Clone SGODataPoint
            if (newSgoId > 0)
            {
                var lst = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId);
                if (lst != null && lst.Any())
                {
                    foreach (var dataPoint in lst)
                    {
                        //Call store create SGODataPoint
                        _parameters.SGOObjectService.SGORelatedDataPoint(dataPoint.SGODataPointID, newSgoId);
                    }
                }
            }

            return newSgoId;
        }

        #endregion

        public ActionResult GetColorGroups(int totalGroups)
        {
            var listColor = ColorHelper.GetColorHexList(totalGroups, true);
            var model = new List<ListItem>();
            for (int i = 0; i < listColor.Count; i++)
            {
                model.Add(new ListItem()
                {
                    Id = i + 1,
                    Name = listColor[i]
                });
            }
            return Json(new { success = true, colors = model }, JsonRequestBehavior.AllowGet);
        }

        #region Add New Students

        public ActionResult LoadAddNewStudentsFilter(int id)
        {
            var model = new SGOAddNewStudentsCustomViewModel
            {
                SGOID = id,
                IsPublisherOrNetworkAdmin = false,
                IsPublisher = CurrentUser.IsPublisher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin
            };

            model.IsPublisherOrNetworkAdmin = CurrentUser.IsPublisher() || CurrentUser.IsNetworkAdmin();

            var sgo = _parameters.SGOObjectService.GetSGOByID(id);
            if (sgo != null)
            {
                model.SGOStatusID = sgo.SGOStatusID;
            }

            return PartialView("_AddNewStudentsFilter", model);
        }

        public ActionResult AddNewStudentsFilter(int id)
        {
            var model = new SGOAddNewStudentsCustomViewModel()
            {
                SGOID = id,
                IsPublisherOrNetworkAdmin = false,
                IsPublisher = CurrentUser.IsPublisher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin
            };
            if (CurrentUser.IsPublisher() || CurrentUser.IsNetworkAdmin())
                model.IsPublisherOrNetworkAdmin = true;

            return View("_AddNewStudentsFilter", model);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AddStudentsToSGO(AddStudentsToSGOModel model)
        {
            if (model == null || !model.SGOID.HasValue || model.StudentIDs == null || model.StudentIDs.Count == 0)
            {
                return Json(new { Status = "false" }, JsonRequestBehavior.AllowGet);
            }

            var objSgo = _parameters.SGOObjectService.GetSGOByID(model.SGOID.Value);
            var vPermissionAccess = _parameters.SGOObjectService.GetPermissionAccessSgoPreparednessGroup(CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate
                && vPermissionAccess.Status != (int)SGOPermissionEnum.MinorUpdate)
            {
                return Json(new { Status = "false" }, JsonRequestBehavior.AllowGet);
            }

            var SGOStudentList = new List<SGOStudent>();
            foreach (var studentIdAndClassId in model.StudentIDs)
            {
                var strArr = studentIdAndClassId.Split(';');
                var studentId = 0;
                var classId = 0;
                if (strArr.Length != 2)
                    continue;
                if (!int.TryParse(strArr[0], out studentId) || !int.TryParse(strArr[1], out classId))
                    continue;

                if (studentId > 0 && classId > 0)
                {
                    var sgoStudent = new SGOStudent
                    {
                        Type = 1,
                        SGOID = model.SGOID.Value,
                        ClassID = classId,
                        StudentID = studentId
                    };
                    SGOStudentList.Add(sgoStudent);

                }
            }
            //check security
            //check ajax parameters modify
            if (!_parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser,
                string.Join(",", SGOStudentList.Select(x => x.StudentID).ToList())))
            {
                return Json(new { Status = "false", ErrorMessage = "Has no right on one or more Student." }, JsonRequestBehavior.AllowGet);
            }
            if (!_parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser,
                SGOStudentList.Select(x => x.ClassID).ToList()))
            {
                return Json(new { Status = "false", ErrorMessage = "Has no right on one or more Class." }, JsonRequestBehavior.AllowGet);
            }
            foreach (var SGOStudent in SGOStudentList)
            {
                _parameters.SgoStudentService.SaveSGOStudent(SGOStudent);
            }

            return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStudentResult(LookupStudentCustom model)
        {
            var parser = new DataTableParserProc<LookupStudentViewModel>();
            var data = new List<LookupStudentViewModel>().AsQueryable();
            int? totalRecords = 0;
            if (!model.ClassId.HasValue)
                model.ClassId = -1;

            if (!model.DistrictId.HasValue && !CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin())
                model.DistrictId = CurrentUser.DistrictId.Value;

            if (model.DistrictId > 0)
            {
                model.UserId = CurrentUser.Id;
                model.RoleId = CurrentUser.RoleId;

                var sortColumns = parser.SortableColumns;

                model.FirstName = model.FirstName == null ? null : model.FirstName.Trim();
                model.LastName = model.LastName == null ? null : model.LastName.Trim();
                model.Code = model.Code == null ? null : model.Code.Trim();
                model.StateCode = model.StateCode == null ? null : model.StateCode.Trim();

                var userSchools = _parameters.UserSchoolServices.GetSchoolsUserHasAccessTo(CurrentUser.Id).ToList();

                data = _parameters.StudentServices.SGOLookupStudents(model, parser.StartIndex, parser.PageSize, ref totalRecords, sortColumns)
                    .Select(x => new LookupStudentViewModel
                    {
                        Code = x.Code,
                        FirstName = x.FirstName,
                        GenderCode = x.GenderCode,
                        GradeName = x.GradeName,
                        LastName = x.LastName,
                        RaceName = x.RaceName,
                        SchoolName = x.SchoolName,
                        StateCode = x.StateCode,
                        StudentId = x.StudentId,
                        Status = x.Status,
                        CanAccess = CanAccessStudentByAdminSchool(x.AdminSchoolId, x.DistrictId, userSchools)
                    }).AsQueryable();
            }

            return Json(parser.Parse(data, totalRecords ?? 0), JsonRequestBehavior.AllowGet);
        }

        private bool CanAccessStudentByAdminSchool(int? adminSchoolId, int? districtId, List<UserSchool> userSchools)
        {
            if (adminSchoolId.HasValue
                && !CurrentUser.IsLinkItAdminOrPublisher()
                && (!CurrentUser.IsDistrictAdminOrPublisher || (CurrentUser.IsDistrictAdminOrPublisher && CurrentUser.DistrictId != districtId))
                )
            {
                if (userSchools.All(en => en.SchoolId != adminSchoolId))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Final Signoff

        public ActionResult FinalSignOff(int id)
        {
            var objSgo = _parameters.SGOObjectService.GetSGOByID(id);
            if (objSgo == null)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            var vPermissionAccess = _parameters.SGOObjectService.GetPermissionAccessSgoFinalSignoff(CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status == (int)SGOPermissionEnum.NotAvalible)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            var model = new FinalSignOffViewModel()
            {
                SGOId = objSgo.SGOID,
                PermissionAccess = vPermissionAccess.Status,
                IsApprover = objSgo.OwnerUserID != CurrentUser.Id,
                SGOStatusId = objSgo.SGOStatusID,
                AdminComments = objSgo.AdminComment,
                TeacherComments = objSgo.TeacherComment
            };
            model.comments = objSgo.OwnerUserID == CurrentUser.Id ? objSgo.TeacherComment : objSgo.AdminComment;

            var vDistrictDecode = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(objSgo.DistrictID, Constanst.SGOFinalSignoffDirection);
            if (vDistrictDecode.Any())
            {
                model.FinalSignoffDirection = vDistrictDecode.First().Value;
            }
            return View(model);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult SGOFinalSignOffApprove(int sgoId, string strComment)
        {
            var obj = _parameters.SGOObjectService.GetSGOByID(sgoId);
            if (obj == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            if (obj.SGOStatusID != (int)SGOStatusType.EvaluationSubmittedForApproval)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            //GetAccessPermission(sgo.DistrictID, userId, sgo.SGOID)

            if (!_parameters.SGOObjectService.GetAccessPermission(obj.DistrictID, CurrentUser.Id, obj.SGOID) || obj.OwnerUserID == CurrentUser.Id)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            _parameters.SGOMilestoneService.CreateMilestoneWithStatus(obj.SGOID, CurrentUser.Id,
                (int)SGOStatusType.SGOApproved);
            obj.AdminComment = strComment.Trim();
            obj.SGOStatusID = (int)SGOStatusType.SGOApproved;
            _parameters.SGOObjectService.Save(obj);
            SendFinalSignOffEmail(obj, false);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult SGOFinalSignOffDeny(int sgoId, string strComment)
        {
            var obj = _parameters.SGOObjectService.GetSGOByID(sgoId);
            if (obj == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            if (obj.SGOStatusID != (int)SGOStatusType.EvaluationSubmittedForApproval)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (!_parameters.SGOObjectService.GetAccessPermission(obj.DistrictID, CurrentUser.Id, obj.SGOID) || obj.OwnerUserID == CurrentUser.Id)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            obj.SGOStatusID = (int)SGOStatusType.SGODenied;
            obj.AdminComment = strComment.Trim();
            _parameters.SGOObjectService.Save(obj);
            CloneSGOAndUpdateVersion(sgoId, (int)SGOStatusType.SGODenied);
            SendFinalSignOffEmail(obj, false);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult SGOTeacherAcknowledged(int sgoId, string strComment)
        {
            var obj = _parameters.SGOObjectService.GetSGOByID(sgoId);

            if (obj != null)
            {
                var vPermissionAccess = _parameters.SGOObjectService.GetPermissionAccessSgoFinalSignoff(CurrentUser.Id, obj);
                if (vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate)
                    return Json(false, JsonRequestBehavior.AllowGet);

                _parameters.SGOMilestoneService.CreateMilestoneWithStatus(obj.SGOID, CurrentUser.Id, (int)SGOStatusType.TeacherAcknowledged);
                obj.SGOStatusID = (int)SGOStatusType.TeacherAcknowledged;
                obj.TeacherComment = strComment.Trim();
                _parameters.SGOObjectService.Save(obj);
                SendFinalSignOffEmail(obj, true);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        private void SendFinalSignOffEmail(SGOObject obj, bool isTeacherAcknowledged)
        {
            string strBody = "Body SGO FinalSignOff";
            string strSubject = "SGO FinalSignOff";
            var objEmailTemplate = new DistrictDecode();
            if (!isTeacherAcknowledged)
            {
                if (obj.SGOStatusID == (int)SGOStatusType.SGOApproved)
                {
                    strSubject = "Approved";
                    objEmailTemplate = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(obj.DistrictID, Constanst.Configuration_SGOEmailTemplateAdminApproval).FirstOrDefault();
                }
                else if (obj.SGOStatusID == (int)SGOStatusType.SGODenied)
                {
                    strSubject = "Denied";
                    objEmailTemplate = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(obj.DistrictID, Constanst.Configuration_SGOEmailTemplateAdminDeny).FirstOrDefault();
                }
            }
            else
            {
                strSubject = "Teacher Acknowledged";
                objEmailTemplate = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(obj.DistrictID, Constanst.Configuration_SGOEmailTemplateTeacherAcknowledged).FirstOrDefault();
            }

            if (objEmailTemplate != null && objEmailTemplate.Value.HasValue())
            {
                strBody = objEmailTemplate.Value;
            }
            strBody = strBody.Replace("<SGOName>", obj.Name);
            var vUser = _parameters.UserService.GetUserById(obj.OwnerUserID);
            var vReviewUser = _parameters.UserService.GetUserById(obj.ApproverUserID.GetValueOrDefault());
            if (vUser != null && vReviewUser != null)
            {
                string strEmail = vReviewUser.EmailAddress;
                if (!isTeacherAcknowledged)
                {
                    strEmail = vUser.EmailAddress;
                    strBody = strBody.Replace("<ApprovedUser>", string.Format("{0}, {1}", vReviewUser.LastName, vReviewUser.FirstName));
                    strBody = strBody.Replace("<comments>", WebUtility.HtmlEncode(obj.AdminComment).Replace("\n", "<br />"));
                }
                else
                {
                    strBody = strBody.Replace("<TeacherName>", string.Format("{0}, {1}", vUser.LastName, vUser.FirstName));
                    strBody = strBody.Replace("<comments>", WebUtility.HtmlEncode(obj.TeacherComment).Replace("\n", "<br />"));
                }
                Util.SendMailSGO(strBody, strSubject, strEmail);
            }
        }
        #endregion

        public ActionResult GetCustomAssessmentByRule(int sgoId, string hasCustomAssessment)
        {
            var vDataPointInvalid = _parameters.SGOObjectService.GetDataPointHasNoBand(sgoId);
            string strNumeric = string.Empty;
            string strAlphaNumeric = string.Empty;
            var vValidDataPointHistorical = true;
            var invalidHistoricalDefaultBandHasFilters = string.Empty;
            if (vDataPointInvalid != null && vDataPointInvalid.Count > 0)
            {
                if (hasCustomAssessment.ToUpper() == "TRUE")
                {
                    int[] arrNumeric = { 1, 2, 3, 4, 5, 6, 7, 8 };
                    int[] arrAlphaNumeric = { 9, 10 };
                    foreach (var dt in vDataPointInvalid)
                    {
                        if (dt.ScoreType.HasValue && arrNumeric.Contains(dt.ScoreType.Value))
                        {
                            strNumeric = string.Format("{0}, {1}", strNumeric, dt.Name);
                        }
                        else if (dt.ScoreType.HasValue && arrAlphaNumeric.Contains(dt.ScoreType.Value))
                        {
                            strAlphaNumeric = string.Format("{0}, {1}", strAlphaNumeric, dt.Name);
                        }
                    }
                    if (strNumeric.StartsWith(", "))
                        strNumeric = strNumeric.Substring(2);
                    if (strAlphaNumeric.StartsWith(", "))
                        strAlphaNumeric = strAlphaNumeric.Substring(2);
                }
                vValidDataPointHistorical = !vDataPointInvalid.Any(o => o.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical);
            }
            invalidHistoricalDefaultBandHasFilters = ValidateHitoricalDataPointDefaultCutScoreHasFilters(sgoId);
            return Json(new { success = true, dtNumeric = strNumeric, dtAlphanumeric = strAlphaNumeric, ValidateDefaultPointBand = vValidDataPointHistorical, InvalidDefaultBandHasFilters = invalidHistoricalDefaultBandHasFilters }, JsonRequestBehavior.AllowGet);
        }

        private string ValidateHitoricalDataPointDefaultCutScoreHasFilters(int sgoId)
        {
            var sgoDataPoints =
                _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId)
                    .Where(x => x.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical)
                    .ToList();
            foreach (var dataPoint in sgoDataPoints)
            {
                if (!_parameters.SgoDataPointService.CheckIsCustomCutScore(dataPoint) &&
                    _parameters.SgoDataPointService.GetDataPointClusterFilterCount(dataPoint.SGODataPointID) > 1)
                {
                    return dataPoint.Name;
                }
            }
            return string.Empty;
        }

        public List<ListItemStr> GetCustomScoreType(SGODataPoint sgoDataPoint, int districtId)
        {
            var scoreTypes = new List<ListItemStr>();
            if (sgoDataPoint == null) return scoreTypes;

            int? customScoreId = null;

            if (sgoDataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom)
            {
                customScoreId = sgoDataPoint.AchievementLevelSettingID.GetValueOrDefault();
            }

            var isPostAssignment = sgoDataPoint.Type == 1 || sgoDataPoint.Type == 5 || sgoDataPoint.Type > 6;

            var virtualTestCustomScore =
                _parameters.SgoSelectDataPointService.SGOGetAssessmentScoreType(sgoDataPoint.VirtualTestID.GetValueOrDefault(), districtId, customScoreId, sgoDataPoint.SGOID, isPostAssignment, sgoDataPoint.Type);
            if (virtualTestCustomScore.UsePercent)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScorePercent).ToString(), Name = "Score Percent" });

            if (virtualTestCustomScore.UsePercentile)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScorePercentage).ToString(), Name = "Score Percentage" });

            if (virtualTestCustomScore.UseRaw)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreRaw).ToString(), Name = "Score Raw" });

            if (virtualTestCustomScore.UseScaled)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreScaled).ToString(), Name = "Score Scaled" });

            if (virtualTestCustomScore.UseIndex)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreIndex).ToString(), Name = "Score Index" });

            if (virtualTestCustomScore.UseLexile)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreLexile).ToString(), Name = "Score Lexile" });

            if (virtualTestCustomScore.UseCustomN1 ?? false)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomN1).ToString(), Name = virtualTestCustomScore.CustomN1Label ?? SGOScoreTypeEnum.ScoreCustomN1.DescriptionAttr() });

            if (virtualTestCustomScore.UseCustomN2 ?? false)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomN2).ToString(), Name = virtualTestCustomScore.CustomN2Label ?? SGOScoreTypeEnum.ScoreCustomN2.DescriptionAttr() });

            if (virtualTestCustomScore.UseCustomN3 ?? false)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomN3).ToString(), Name = virtualTestCustomScore.CustomN3Label ?? SGOScoreTypeEnum.ScoreCustomN3.DescriptionAttr() });

            if (virtualTestCustomScore.UseCustomN4 ?? false)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomN4).ToString(), Name = virtualTestCustomScore.CustomN4Label ?? SGOScoreTypeEnum.ScoreCustomN4.DescriptionAttr() });

            if (virtualTestCustomScore.UseCustomA1 ?? false)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomA1).ToString(), Name = virtualTestCustomScore.CustomA1Label ?? SGOScoreTypeEnum.ScoreCustomA1.DescriptionAttr() });

            if (virtualTestCustomScore.UseCustomA2 ?? false)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomA2).ToString(), Name = virtualTestCustomScore.CustomA2Label ?? SGOScoreTypeEnum.ScoreCustomA2.DescriptionAttr() });

            if (virtualTestCustomScore.UseCustomA3 ?? false)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomA3).ToString(), Name = virtualTestCustomScore.CustomA3Label ?? SGOScoreTypeEnum.ScoreCustomA3.DescriptionAttr() });

            if (virtualTestCustomScore.UseCustomA4 ?? false)
                scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomA4).ToString(), Name = virtualTestCustomScore.CustomA4Label ?? SGOScoreTypeEnum.ScoreCustomA4.DescriptionAttr() });

            var virtualTestCustomSubScores = _parameters.VirtualTestCustomSubScoreService.Select().Where(x => x.VirtualTestCustomScoreId == customScoreId).OrderBy(x => x.Sequence);
            foreach (var virtualTestCustomSubScore in virtualTestCustomSubScores)
            {
                var subScoreTypes = BuildSubScoreType(virtualTestCustomSubScore);
                scoreTypes.AddRange(subScoreTypes);
            }

            return scoreTypes;
        }
        private List<ListItemStr> BuildSubScoreType(VirtualTestCustomSubScore virtualTestCustomSubScore)
        {
            var data = new List<ListItemStr>();
            if (virtualTestCustomSubScore.UseRaw)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreRaw, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - Score Raw", virtualTestCustomSubScore.Name) });

            if (virtualTestCustomSubScore.UseScaled)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreScaled, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - Score Scaled", virtualTestCustomSubScore.Name) });

            if (virtualTestCustomSubScore.UsePercentile)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScorePercentage, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - Score Percentage", virtualTestCustomSubScore.Name) });

            if (virtualTestCustomSubScore.UsePercent)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScorePercent, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - Score Percent", virtualTestCustomSubScore.Name) });

            if (virtualTestCustomSubScore.UseIndex)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreIndex, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - Score Index", virtualTestCustomSubScore.Name) });

            if (virtualTestCustomSubScore.UseLexile)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreLexile, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - Score Lexile", virtualTestCustomSubScore.Name) });

            if (virtualTestCustomSubScore.UseCustomN1 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomN1, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomN1Label) });

            if (virtualTestCustomSubScore.UseCustomN2 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomN2, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomN2Label) });

            if (virtualTestCustomSubScore.UseCustomN3 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomN3, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomN3Label) });

            if (virtualTestCustomSubScore.UseCustomN4 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomN4, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomN4Label) });

            if (virtualTestCustomSubScore.UseCustomA1 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomA1, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomA1Label) });

            if (virtualTestCustomSubScore.UseCustomA2 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomA2, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomA2Label) });

            if (virtualTestCustomSubScore.UseCustomA3 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomA3, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomA3Label) });

            if (virtualTestCustomSubScore.UseCustomA4 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomA4, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomA4Label) });

            return data;
        }
        public ActionResult LoadPrintConfirm(int sgoId)
        {
            var obj = _parameters.SGOObjectService.GetSGOByID(sgoId);
            if (obj == null)
            {
                return PartialView("_SGOPrintConfirm", new ObjectCommonCustomViewModel());
            }
            var model = new ObjectCommonCustomViewModel()
            {
                Id = sgoId,
                Name = Regex.Replace(obj.Name.Trim(), "[^A-Za-z0-9]+", "-"),
                ExtraId = CurrentUser.Id
            };
            return PartialView("_SGOPrintConfirm", model);
        }

        public ActionResult LoadCandidateClassForReplacement(int sgoId)
        {
            var model = new SGOLoadCandidateClassForReplacementViewModel();
            model.SGOCandidateClasses = _parameters.SGOObjectService.GetCandidateClassForReplacement(sgoId);
            model.SGOStudents = _parameters.SGOObjectService.GetAllStudentsBySogId(sgoId);

            for (int i = model.SGOCandidateClasses.Count() - 1; i >= 0; i--)
            {
                if (!_parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, model.SGOCandidateClasses[i].CandidateClassId))
                {
                    model.SGOCandidateClasses.Remove(model.SGOCandidateClasses[i]);
                }
            }

            return PartialView("_LoadCandidateClassForReplacement", model);
        }

        public ActionResult ApplyCandidateClassForReplacement(int sgoId, string replacementInfo)
        {

            var replacementItems = replacementInfo.Split(';');
            foreach (var item in replacementItems)
            {
                try
                {
                    var removedClassId = Convert.ToInt32(item.Split('|')[0]);
                    var candidateClassId = Convert.ToInt32(item.Split('|')[1]);

                    _parameters.SGOObjectService.ApplyCandidateClassForReplacement(sgoId, removedClassId, candidateClassId);
                }
                catch
                {
                    //nothing 
                }
            }

            // Sync selected class data from SGOStudentFilter into SGO.ClassIDs and SGOSchoolIDs fields
            _parameters.SGOObjectService.PopulateSchoolIdsAndClassIdsBySgoId(sgoId);

            return Json(new { sucess = true }, JsonRequestBehavior.AllowGet);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [AjaxOnly]
        public ActionResult SaveNote(int sgoId, string pageName, string sgoNote)
        {
            _parameters.SGOObjectService.SaveNote(sgoId, pageName, sgoNote);
            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadSGONote(int sgoId, string pageName, bool? layoutV2 = false)
        {
            ViewBag.SGOId = sgoId;
            ViewBag.PageName = pageName;
            ViewBag.SGONote = _parameters.SGOObjectService.GetNote(sgoId, pageName);
            if (layoutV2 == true)
            {
                return PartialView("v2/_SGONote");
            }
            return PartialView("_SGONote");
        }

        public ActionResult GetDefaultInstructionPeriod(int districtID)
        {
            var objPreference = _parameters.PreferencesService.GetPreferenceByLevelLabelAndId(ContaintUtil.TestPreferenceLevelDistrict, ContaintUtil.TestPreferenceLabelDefaultStartMonth, districtID);
            if (objPreference == null)
            {
                objPreference = _parameters.PreferencesService.GetPreferenceByLevelLabelAndId(ContaintUtil.TestPreferenceLevelEnterprise, ContaintUtil.TestPreferenceLabelDefaultStartMonth, 0);
            }

            var vCurrentMonth = DateTime.UtcNow.Month;
            int iStartMonth = 0;
            int.TryParse(objPreference.Value, out iStartMonth);
            DateTime dtStartDate;
            DateTime dtEndDate;

            var year = DateTime.UtcNow.Year;
            if (vCurrentMonth < iStartMonth)
                year -= 1;

            dtStartDate = new DateTime(year, iStartMonth, 1);
            dtEndDate = new DateTime(year + 1, iStartMonth, 1);
            dtEndDate = dtEndDate.AddSeconds(-1);

            var configDateFormat = _parameters.DistrictDecodeService.GetDateFormat(districtID);

            return Json(new { Success = true, dateFrom = dtStartDate.ToString(configDateFormat.DateFormat), dateTo = dtEndDate.ToString(configDateFormat.DateFormat) }, JsonRequestBehavior.AllowGet);
        }

        #region Export final-sign off SGO for district admin

        [HttpGet]
        public ActionResult ExportTeacherData(SGOHomeFilterViewModel model)
        {
            if (!CurrentUser.IsDistrictAdmin && !CurrentUser.IsSchoolAdmin)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var sgoToExport = _parameters.SGOObjectService.GetFinalAdministrativeSignoffSGOByUserId(CurrentUser.Id, model.InstructionPeriodFrom, model.InstructionPeriodTo, model.IsArchivedStatusArchived, model.IsArchivedStatusActive, model.SGOStatusIds);
            if (sgoToExport.Any())
            {
                var result = ExportSgoToCvs(sgoToExport);
                return File(new UTF8Encoding().GetBytes(result), "text/csv", "ExportTeacherData.csv");
            }
            else
            {
                TempData["IsSignedOffSGO"] = true;
                return RedirectToAction("Index");
            }
        }

        private string ExportSgoToCvs(List<SGOExportData> sgoToExport)
        {
            var ms = new MemoryStream();
            TextWriter textWriter = null;
            CsvWriter csvWriter = null;
            try
            {
                foreach (var item in sgoToExport)
                {
                    if (item.SchoolName == null)
                        item.SchoolName = "";
                }

                textWriter = new StreamWriter(ms);
                csvWriter = new CsvWriter(textWriter);
                var streamReader = new StreamReader(ms);
                var reportRecords = sgoToExport.Select(x => new { x.UserID, x.SchoolName }).Distinct().ToList();
                WriteHeader(csvWriter, sgoToExport);
                foreach (var reportRecord in reportRecords)
                {
                    var recordItems = sgoToExport.Where(x => x.UserID.Equals(reportRecord.UserID) && x.SchoolName.Equals(reportRecord.SchoolName)).ToList();
                    if (recordItems.Any())
                    {
                        Func<SGOExportData, bool> notCustomCondition =
                            x => x.Type == (int)SGOTypeEnum.Normal;
                        var notCustomItems = recordItems.Where(notCustomCondition);                               
                        
                        var customItems = recordItems.Where(x => x.Type == (int)SGOTypeEnum.UnstructuredData);                        
                        foreach(var item in customItems)
                        { 
                            decimal customScore;
                            if (decimal.TryParse(item.ScoreCustom, out customScore))
                                item.TargetScore = customScore;
                        }
                        customItems = customItems.Where(x => x.TargetScore.HasValue);

                        var avgScore = (notCustomItems.Any() || customItems.Any()) ? notCustomItems.Union(customItems).Select(x => x.TargetScore.GetValueOrDefault()).Average() : 0;
                        avgScore = Math.Round(avgScore, 3);

                        WriteGeneralInformation(csvWriter, avgScore, recordItems[0]);
                        foreach (var sgoObject in recordItems)
                        {
                            WriteSgoInformation(csvWriter, sgoObject);
                        }
                        csvWriter.NextRecord();
                    }
                }
                textWriter.Flush();
                ms.Position = 0;
                return streamReader.ReadToEnd();
            }
            finally
            {
                if (textWriter != null) textWriter.Dispose();
                if (csvWriter != null) csvWriter.Dispose();
            }
        }

        private void WriteHeader(CsvWriter csvWriter, List<SGOExportData> sgoToExport)
        {
            var maxSgoCount =
                sgoToExport.GroupBy(x => new { x.UserID, x.SchoolName })
                    .Select(x => new { sgocount = x.Count() })
                    .Max(x => x.sgocount);
            csvWriter.WriteField("State ID");
            csvWriter.WriteField("Average SGO Score");
            csvWriter.WriteField("School");
            csvWriter.WriteField("First Name");
            csvWriter.WriteField("Last Name");
            csvWriter.WriteField("SISID");
            csvWriter.WriteField("LocalID");
            for (int i = 0; i < maxSgoCount; i++)
            {
                csvWriter.WriteField(string.Format("SGO {0} Name", i + 1));
                csvWriter.WriteField(string.Format("SGO {0} Score", i + 1));
                csvWriter.WriteField(string.Format("SGO {0} Final Signoff Date", i + 1));
            }
            csvWriter.NextRecord();
        }

        private void WriteSgoInformation(CsvWriter csvWriter, SGOExportData sgoObject)
        {
            csvWriter.WriteField(sgoObject.Name);
            if (sgoObject.Type == (int)SGOTypeEnum.UnstructuredData)
            {
                csvWriter.WriteField(sgoObject.ScoreCustom ?? string.Empty);
            }
            else
            {
                csvWriter.WriteField(string.Format("{0:0.000}", Math.Round(sgoObject.TargetScore.GetValueOrDefault(), 3)));
            }

            csvWriter.WriteField(sgoObject.FinalSignoffDate.GetValueOrDefault().ToString("d"));
        }

        private void WriteGeneralInformation(CsvWriter csvWriter, decimal avgScore, SGOExportData sgoObject)
        {
            csvWriter.WriteField(sgoObject.StateId ?? string.Empty);
            csvWriter.WriteField(string.Format("{0:0.000}", avgScore));
            csvWriter.WriteField(sgoObject.SchoolName ?? string.Empty);
            csvWriter.WriteField(sgoObject.FirstName ?? string.Empty);
            csvWriter.WriteField(sgoObject.LastName ?? string.Empty);
            csvWriter.WriteField(sgoObject.SISID ?? string.Empty);
            csvWriter.WriteField(sgoObject.LocalCode ?? string.Empty);
        }

        #endregion

    }
}
