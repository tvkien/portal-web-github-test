using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Data.Linq;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.BusinessObjects;
using LinkIt.BubbleSheetPortal.Common;
using System.Threading.Tasks;
using LinkIt.BubbleSheetPortal.Models.RestrictionDTO;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class PopulateTestController : BaseController
    {
        private readonly GradeService gradeService;
        private readonly SubjectService subjectService;
        private readonly TestService testService;
        private readonly DistrictService districtService;
        private readonly UserBankService userBankService;
        private readonly IncorrectQuestionOrderService incorrectQuestionOrderService;
        private readonly RestrictionBO _restrictionBO;
        private readonly ManageTestService _manageTestService;
        private readonly VirtualTestService _virtualTestService;
        private readonly DistrictDecodeService _districtDecodeService;

        public PopulateTestController(GradeService gradeService,
            SubjectService subjectService,
            TestService testService,
            DistrictService districtService,
            UserBankService userBankService,
            IncorrectQuestionOrderService incorrectQuestionOrderService,
            RestrictionBO restrictionBO,
            ManageTestService manageTestService,
            VirtualTestService virtualTestService,
            DistrictDecodeService districtDecodeService)
        {
            this.gradeService = gradeService;
            this.subjectService = subjectService;
            this.testService = testService;
            this.districtService = districtService;
            this.userBankService = userBankService;
            this.incorrectQuestionOrderService = incorrectQuestionOrderService;
            _restrictionBO = restrictionBO;
            _manageTestService = manageTestService;
            _virtualTestService = virtualTestService;
            _districtDecodeService = districtDecodeService;
        }

        [HttpGet]
        public ActionResult GetGrades()
        {
            var data = gradeService.GetGrades().Select(x => new ListItem { Id = x.Id, Name = x.Name }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetGradesByDistrict_old(int districtId)
        {
            if (districtId > 0)
            {
                if (!Util.HasRightOnDistrict(CurrentUser, districtId))
                {
                    return Json(new { error = "Has no right on district" }, JsonRequestBehavior.AllowGet);
                }
            }
            var tmp = gradeService.GetGradesByUserIdDistrictIdRoleId(CurrentUser, districtId);
            if (tmp != null && tmp.Any())
            {
                var data = tmp.Select(x => new ListItem() { Id = x.Id, Name = x.Name }).ToList();
                var gradeIncludes = _manageTestService.GetGradeIncludes(CurrentUser.Id);
                data.AddRange(gradeIncludes);
                data = data.DistinctBy(m => m.Id).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetGradesFormBankByDistrictId(int districtId, bool? isFromMultiDate)
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on district" }, JsonRequestBehavior.AllowGet);
            }
            if (districtId <= 0)
                districtId = CurrentUser.DistrictId.GetValueOrDefault();

            var usingMultiDate = _districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.UseMultiDateTemplate);
            var tmp = gradeService.GetGradesFormBankByUserIdDistrictIdRoleId(CurrentUser, districtId, isFromMultiDate, usingMultiDate);
            var gradeOrders = gradeService.GetGradeOrders(districtId < 0 ? CurrentUser.DistrictId.GetValueOrDefault() : districtId);

            if (tmp != null && tmp.Any())
            {
                if (!gradeOrders.Any())
                {
                    tmp = tmp.OrderBy(x => x.Order).ToList();
                    var data = tmp.Select(x => new ListItem() { Id = x.Id, Name = x.Name }).ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var sortedGrades = SortGrades(tmp, gradeOrders);
                    return Json(sortedGrades, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Grade by State for TestManage
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetGradesByStateId(int stateId)
        {
            if (stateId < 0)
            {
                stateId = CurrentUser.StateId.HasValue ? CurrentUser.StateId.Value : 0;
            }
            var data = gradeService.GetGradesByStateID(stateId).Select(x => new ListItem { Id = x.Id, Name = x.Name });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Subject by State and Grade for TestManage
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="gradeId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetSubjectByStateIdAndGradeId(int stateId, int gradeId)
        {
            var data = subjectService.GetSubjectsByGradeAndState(gradeId, stateId).Select(o => new ListItem()
            {
                Id = o.Id,
                Name = o.Name
            }).OrderBy(o => o.Name);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSubjects(int gradeId, int? districtId)
        {
            var userDistrictId = CurrentUser.IsPublisher() ? districtId : CurrentUser.DistrictId;
            var data = GetSubjects(gradeId, userDistrictId.GetValueOrDefault());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSubjectsFormBank(int gradeId, int? districtId, bool? isFromMultiDate)
        {
            if (!districtId.HasValue || districtId.Value <= 0)
                districtId = CurrentUser.DistrictId;
            var usingMultiDate = _districtDecodeService.GetDistrictDecodeByLabel(districtId.GetValueOrDefault(), Constanst.UseMultiDateTemplate);

            var data = GetSubjectsFormBank(gradeId, districtId.GetValueOrDefault(), isFromMultiDate, usingMultiDate);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetSubjectsGroupedByNameFormBank(int gradeId, int districtId = 0, bool isFromMultiDate = false)
        {
            int currentDistrictId = 0;
            if (!(CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin))
                currentDistrictId = CurrentUser.DistrictId ?? 0;
            else
                currentDistrictId = districtId;

            var usingMultiDate = _districtDecodeService.GetDistrictDecodeByLabel(currentDistrictId, Constanst.UseMultiDateTemplate);

            var subjects = GetSubjectsFormBank(gradeId, currentDistrictId, isFromMultiDate, usingMultiDate);
            var subjectGrouped = subjects.GroupBy(subject => subject.Name)
                .Select(group => new ListItemStr()
                {
                    Id = string.Join(",", group.Select(item => item.Id).ToArray()),
                    Name = group.Key
                }).ToArray();
            return Json(subjectGrouped, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetBanks(int subjectId)
        {
            var data = userBankService.GetUserBanksBySubjectAndUser(subjectId, CurrentUser.Id).Select(x => new ListItem { Id = x.Id, Name = x.BankName }).OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTests(int bankId, int? districtId, string moduleCode = "")
        {
            var data = testService.GetValidTestsByBank(new List<int>() { bankId }).Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();

            if (!string.IsNullOrEmpty(moduleCode))
            {
                if (!districtId.HasValue || districtId.Value == 0) districtId = CurrentUser.DistrictId.GetValueOrDefault();
                data = _restrictionBO.FilterTests(new Models.RestrictionDTO.FilterTestQueryDTO
                {
                    BankId = bankId,
                    DistrictId = districtId.Value,
                    ModuleCode = moduleCode,
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                    Tests = data
                });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetTestsCustomOrder(GetTestsCustomOrderRequest request)
        {
            var virtualTestsTemp = testService.GetValidTestsByBank(new List<int>() { request.BankID }, request.IsIncludeRetake);
            var data = virtualTestsTemp.Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();

            if (!request.DistrictId.HasValue || request.DistrictId.Value == 0) request.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();

            if (!string.IsNullOrEmpty(request.ModuleCode))
            {
                data = _restrictionBO.FilterTests(new Models.RestrictionDTO.FilterTestQueryDTO
                {
                    BankId = request.BankID,
                    DistrictId = request.DistrictId.Value,
                    ModuleCode = request.ModuleCode,
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                    Tests = data
                });
            }
            var virtualTestOrders = _virtualTestService.GetVirtualTestOrders(request.DistrictId.GetValueOrDefault());
            if (virtualTestOrders.Any())
            {
                var maxOrder = virtualTestOrders.Max(x => x.Order) + 100;

                var query = from test in data
                            join order in virtualTestOrders on test.Id equals order.VirtualTestID into ps
                            from p in ps.DefaultIfEmpty()
                            select new { test.Id, test.Name, Order = p == null ? maxOrder : p.Order };

                var result = query.OrderBy(x => x.Order).ThenBy(x => x.Name).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestsForTestAssignmentCustomOrder(int bankId, int districtId = 0, string moduleCode = "", bool isSurvey = false)
        {
            var listTest = (isSurvey ? testService.GetValidSurveyByBank(bankId) : testService.GetValidTestsByBank(new List<int>() { bankId }));
            var data = listTest.Select(x => new { Id = x.Id, Name = x.Name, x.DataSetOriginID, x.ParentTestID, x.OriginalTestID, IsTeacherLed = x.IsTeacherLed.GetValueOrDefault(), TotalQuestionGroup = x.QuestionGroupCount }).OrderBy(x => x.Name).ToList();

            if (districtId == 0)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }

            if (!string.IsNullOrEmpty(moduleCode))
            {
                var allowTestIds = _restrictionBO.FilterTests(new Models.RestrictionDTO.FilterTestQueryDTO
                {
                    BankId = bankId,
                    DistrictId = districtId,
                    RoleId = CurrentUser.RoleId,
                    UserId = CurrentUser.Id,
                    ModuleCode = moduleCode,
                    Tests = data.Select(m => new ListItem { Id = m.Id, Name = m.Name }).ToList(),
                }).Select(m => m.Id);

                data = data.Where(m => allowTestIds.Contains(m.Id)).ToList();
            }

            var virtualTestOrders = _virtualTestService.GetVirtualTestOrders(districtId);
            if (virtualTestOrders.Any())
            {
                var maxOrder = virtualTestOrders.Max(x => x.Order) + 100;

                var query = from test in data
                            join order in virtualTestOrders on test.Id equals order.VirtualTestID into ps
                            from p in ps.DefaultIfEmpty()
                            select new { test.Id, test.Name, test.IsTeacherLed, test.TotalQuestionGroup, Order = p == null ? maxOrder : p.Order };

                var result = query.OrderBy(x => x.Order).ThenBy(x => x.Name).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestsForTestAssignmentCustomOrderByAdvancedFilter (GetTestByAdvanceFilterRequest request)
        {
            //Restrict
            request.DistrictID = CurrentDistrict(request.DistrictID);

            var restrictions = _restrictionBO.GetRestrictionList(request.ModuleCode, CurrentUser.Id, CurrentUser.RoleId, PublishLevelTypeEnum.District, request.DistrictID);

            var excludeTestIds = restrictions.Where(r => r.RestrictionObjectType == RestrictionObjectType.Test).Select(r => r.RestrictionObjectId).ToList();

            request.Filters.ExcludedVirtualTestIds = excludeTestIds;

            var banks = userBankService.GetBanksBySubjectNamesAndGradeIDs(new SearchBankAdvancedFilter
            {
                DistrictId = request.DistrictID,
                UserId = CurrentUser.Id,
                UserRole = CurrentUser.RoleId,
                SubjectNames = request.Filters.SubjectNames ?? new List<string>(),
                GradeIds = request.Filters.GradeIds ?? new List<int>()
            }).ToList();

            var banksFinal = new List<ListItem>();
            banksFinal = banks.Select(b => new ListItem
            {
                Id = b.Id,
                Name = b.BankName
            }).ToList();

            if (!string.IsNullOrEmpty(request.ModuleCode))
            {
                banksFinal = _restrictionBO.FilterBanks(new Models.RestrictionDTO.FilterBankQueryDTO
                {
                    Banks = banksFinal,
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                    DistrictId = request.DistrictID,
                    ModuleCode = request.ModuleCode
                }, restrictions);
            }

            if (request.Filters.BankIds.Any())
            {
                request.Filters.BankIds = request.Filters.BankIds.Intersect(banksFinal.Select(b => b.Id).ToList());
            }
            else
            {
                request.Filters.BankIds = banksFinal.Select(b => b.Id);
            }

            var result = _virtualTestService.GetTestByAdvanceFilter(request);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetNonACTTests(int bankId, int? districtId)
        {
            var data = testService.GetNonACTValidTestsByBank(bankId).Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);
            if (!districtId.HasValue || districtId.Value == 0) districtId = CurrentUser.DistrictId.GetValueOrDefault();
            var virtualTestOrders = _virtualTestService.GetVirtualTestOrders(districtId.GetValueOrDefault());
            if (virtualTestOrders.Any())
            {
                var maxOrder = virtualTestOrders.Max(x => x.Order) + 100;

                var query = from test in data
                            join order in virtualTestOrders on test.Id equals order.VirtualTestID into ps
                            from p in ps.DefaultIfEmpty()
                            select new { test.Id, test.Name, Order = p == null ? maxOrder : p.Order };

                var result = query.OrderBy(x => x.Order).ThenBy(x => x.Name).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<ListItem> GetSubjects(int gradeId, int districtId)
        {
            var state = districtService.GetDistrictById(districtId);
            if (state.IsNull())
            {
                return new List<ListItem>();
            }
            return subjectService.GetSubjectsByGradeAndState(gradeId, state.StateId).Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);
        }

        public IEnumerable<ListItem> GetSubjectsFormBank(int gradeId, int districtId, bool? isFromMultiDate, bool usingMultiDate)
        {
            var subjects = subjectService.GetSubjectsFormBankByGradeAndDistrictId(gradeId, districtId, CurrentUser.Id, CurrentUser.RoleId, isFromMultiDate, usingMultiDate);
            if (subjects.Count == 0)
            {
                return new List<ListItem>();
            }
            return subjects.Select(x => new ListItem { Id = x.Id, Name = x.Name });
        }

        public ActionResult GetSubjectsNew(int gradeId, int districtId)
        {
            int currentDistrictId = 0;
            currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            if (CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin)
            {
                currentDistrictId = districtId;
            }

            var tmp = subjectService.GetSubjectsByGradeId(gradeId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId);
            if (tmp != null)
            {
                var item = tmp.Select(x => new ListItem { Id = x.Id, Name = x.Name }).ToList();
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBanksNew(int subjectId, int districtId)
        {
            int currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            if (CurrentUser.IsPublisher() || CurrentUser.IsNetworkAdmin)
            {
                currentDistrictId = districtId;
            }

            var data = userBankService.GetUserBanksBySubjectId(subjectId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBanksForPrintTest(int subjectId, int districtId)
        {
            int currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            if (CurrentUser.IsPublisher() || CurrentUser.IsNetworkAdmin)
            {
                currentDistrictId = districtId;
            }

            var data = userBankService.GetUserBanks(subjectId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckIfTestRequiresCorrection(int testId, bool? pointPossibleLargeThan25)
        {
            var requiresCorrection = incorrectQuestionOrderService.CheckIfTestRequiresCorrection(testId, pointPossibleLargeThan25);
            return Json(new { Success = requiresCorrection }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetACTTests(int? districtId)
        {
            var data = new ListItem(); //TODO: write store Get ACT Test By DistrictId
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ACTGetGradesByDistrict(int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher())
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            var tmp = gradeService.ACTGetGradesByUserIdDistrictIdRoleId(CurrentUser.Id, currentDistrictId,
                CurrentUser.RoleId);
            if (tmp != null && tmp.Any())
            {
                var data = tmp.Select(x => new ListItem() { Id = x.Id, Name = x.Name }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ACTGetSubjectsNew(int gradeId, int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher())
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var tmp = subjectService.ACTGetSubjectsByGradeId(gradeId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId);
            if (tmp != null)
            {
                var item = tmp.Select(x => new ListItem { Id = x.Id, Name = x.Name }).ToList();
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ACTGetBanksNew(int subjectId, int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher())
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var data = userBankService.ACTGetUserBanksBySubjectId(subjectId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ACTGetTestsNew(int bankId, int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher())
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var data = userBankService.ACTGetTestByBankId(bankId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBanksForItemSetSaveTest(int subjectId, int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher())
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var data = userBankService.GetBanksForItemSetSaveTest(subjectId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSubjectsForItemSetSaveTest(int gradeId, int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher())
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var tmp = subjectService.GetSubjectsForItemSetSaveTest(gradeId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId);
            if (tmp != null)
            {
                var item = tmp.Select(x => new ListItem { Id = x.Id, Name = x.Name }).ToList();
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetGradesForItemSetSaveTest(int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            var tmp = gradeService.GetGradesForItemSetSaveTest(CurrentUser.Id, currentDistrictId,
                CurrentUser.RoleId);
            if (tmp != null && tmp.Any())
            {
                var data = tmp.Select(x => new ListItem() { Id = x.Id, Name = x.Name }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetGradesHasSubjects(int districtId, int? stateId)
        {
            int currentStateId = stateId ?? 0;
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
            {
                currentStateId = CurrentUser.StateId ?? 0;
            }
            var tmp = gradeService.GetGrades().OrderBy(x => x.Order).ToList();
            var gradeHasSubject = new List<Grade>();
            var stateSubjects = subjectService.GetSubjectsByState(currentStateId).ToList();

            foreach (var grade in tmp)
            {
                if (stateSubjects.Any(x => x.GradeId == grade.Id))
                {
                    gradeHasSubject.Add(grade);
                }
            }

            if (gradeHasSubject.Count > 0)
            {
                var data = gradeHasSubject.Select(x => new ListItem() { Id = x.Id, Name = x.Name }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SATGetGradesByDistrict(int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher())
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            var tmp = gradeService.SATGetGradesByUserIdDistrictIdRoleId(CurrentUser.Id, currentDistrictId,
                CurrentUser.RoleId);
            if (tmp != null && tmp.Any())
            {
                var data = tmp.Select(x => new ListItem() { Id = x.Id, Name = x.Name }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SATGetSubjectsNew(int gradeId, int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher())
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var tmp = subjectService.SATGetSubjectsByGradeId(gradeId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId);
            if (tmp != null)
            {
                var item = tmp.GroupBy(s => s.Name).Select(x => new ListSubjectItem() { Id = string.Join(",", x.Select(s => s.Id)), Name = x.Key }).ToList();
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SATGetBanksNew(string subjectIds, int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher())
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var data = new List<ListItem>();
            var subjectIdList = subjectIds.Split(',');
            foreach (var id in subjectIdList)
            {
                int subjectId;
                var result = int.TryParse(id, out subjectId);
                if (result)
                {
                    var temp = userBankService.SATGetUserBanksBySubjectId(subjectId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId).ToList();
                    data.AddRange(temp);
                }
            }
            data = data.OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SATGetTestsNew(int bankId, int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher())
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var data = userBankService.SATGetTestByBankId(bankId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<ListItem> SortGrades(List<Grade> grades, List<GradeOrder> gradeOrders)
        {
            var maxOrder = gradeOrders.Max(x => x.Order) + 100;

            var query = from grd in grades
                        join order in gradeOrders on grd.Id equals order.GradeId into ps
                        from p in ps.DefaultIfEmpty()
                        select new { grd.Id, grd.Name, Order = p == null ? maxOrder : p.Order, GradeOrder = grd.Order };

            var list = query.OrderBy(x => x.Order).ThenBy(x => x.GradeOrder).ToList();

            var result = list.Select(o => new ListItem
            {
                Id = o.Id,
                Name = o.Name
            }).ToList();

            return result;
        }

        public ActionResult GetGradesByDistrict(int districtId)
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on district" }, JsonRequestBehavior.AllowGet);
            }
            var gradeListItems = new List<ListItem>();
            var grades = gradeService.GetGradesByUserId(CurrentUser, districtId);
            if (grades != null && grades.Count > 0)
            {
                gradeListItems.AddRange(grades.Select(x => new ListItem() { Id = x.Id, Name = x.Name }));
            }

            return Json(gradeListItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetGradesByDistrictCustomOrder(int districtId)
        {
            if (districtId > 0 && !Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on district" }, JsonRequestBehavior.AllowGet);
            }

            var gradeListItems = new List<ListItem>();
            var grades = gradeService.GetGradesByUserId(CurrentUser, districtId);
            var gradeOrders = gradeService.GetGradeOrders(districtId < 0 ? CurrentUser.DistrictId.GetValueOrDefault() : districtId);

            if (grades != null && grades.Any())
            {
                gradeListItems = !gradeOrders.Any() ? grades.Select(x => new ListItem() { Id = x.Id, Name = x.Name }).ToList() : SortGrades(grades, gradeOrders);
            }
            return Json(gradeListItems, JsonRequestBehavior.AllowGet);
        }
    }
}
