using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.BusinessObjects;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Services.Survey;
using LinkIt.BubbleSheetPortal.Services.BusinessObjects;
using System;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class SearchBankController : BaseController
    {
        private readonly GradeService gradeService;
        private readonly SubjectService subjectService;
        private readonly TestService testService;
        private readonly DistrictService districtService;
        private readonly UserBankService userBankService;
        private readonly BankService bankService;
        private readonly DistrictDecodeService districtDecodeService;
        private readonly RestrictionBO _restrictionBO;
        private readonly ManageTestService _manageTestService;
        private readonly CategoriesService _categoriesService;
        private readonly VirtualTestService _virtualTestService;

        public SearchBankController(GradeService gradeService,
            SubjectService subjectService,
            TestService testService,
            DistrictService districtService,
            UserBankService userBankService,
            BankService bankService,
            DistrictDecodeService districtDecodeService,
            RestrictionBO restrictionBO,
            ManageTestService manageTestService,
            CategoriesService categoriesService,
            VirtualTestService virtualTestService)
        {
            this.gradeService = gradeService;
            this.subjectService = subjectService;
            this.testService = testService;
            this.districtService = districtService;
            this.userBankService = userBankService;
            this.bankService = bankService;
            this.districtDecodeService = districtDecodeService;
            _restrictionBO = restrictionBO;
            _manageTestService = manageTestService;
            _categoriesService = categoriesService;
            _virtualTestService = virtualTestService;
        }
        [HttpGet]
        public ActionResult GetBanksBySubjectName(SearchBankCriteria criteria)
        {
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                criteria.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            criteria.UserId = CurrentUser.Id;
            criteria.UserRole = CurrentUser.RoleId;

            var data = userBankService.GetUserBanksBySubjectName(criteria).ToList();

            if (!string.IsNullOrEmpty(criteria.ModuleCode))
            {
                data = _restrictionBO.FilterBanks(new Models.RestrictionDTO.FilterBankQueryDTO
                {
                    Banks = data,
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                    DistrictId = criteria.DistrictId,
                    ModuleCode = criteria.ModuleCode
                });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetBanksBySubjectNameCustomOrder(SearchBankCriteria criteria)
        {
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                criteria.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            criteria.UserId = CurrentUser.Id;
            criteria.UserRole = CurrentUser.RoleId;

            var data = userBankService.GetUserBanksBySubjectName(criteria).ToList();

            var bankOrders = bankService.GetBankOrders(criteria.DistrictId).ToList();
            if (bankOrders.Any())
                data = bankService.SortBanks(data, bankOrders);

            if (!string.IsNullOrEmpty(criteria.ModuleCode))
            {
                data = _restrictionBO.FilterBanks(new Models.RestrictionDTO.FilterBankQueryDTO
                {
                    Banks = data,
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                    DistrictId = criteria.DistrictId,
                    ModuleCode = criteria.ModuleCode
                });
            }
            
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetBanksBySubjectNameCustomOrderV2(SearchBankCriteria criteria)
        {
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                criteria.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            criteria.UserId = CurrentUser.Id;
            criteria.UserRole = CurrentUser.RoleId;

            //bank
            var banks = userBankService.GetBanksBySubjectNamesAndGradeIDs(new SearchBankAdvancedFilter
            {
                DistrictId = criteria.DistrictId,
                UserId = CurrentUser.Id,
                UserRole = CurrentUser.RoleId
            }).ToList();

            var banksFinal = new List<ListItem>();
            banksFinal = banks.Select(b => new ListItem
            {
                Id = b.Id,
                Name = b.BankName
            }).ToList();

            var bankOrders = bankService.GetBankOrders(criteria.DistrictId).ToList();
            if (bankOrders.Any())
                banksFinal = bankService.SortBanks(banksFinal, bankOrders);

            if (!string.IsNullOrEmpty(criteria.ModuleCode))
            {
                banksFinal = _restrictionBO.FilterBanks(new Models.RestrictionDTO.FilterBankQueryDTO
                {
                    Banks = banksFinal,
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                    DistrictId = criteria.DistrictId,
                    ModuleCode = criteria.ModuleCode
                });
            }

            //subject
            var subjectIds = banks.Where(b => banksFinal.Select(f => f.Id).Contains(b.Id)).Select(b => b.SubjectId).Distinct().ToList();
            var subjects = subjectService.GetSubjectsByListSubjectId(subjectIds);
            var subjectsFinal = subjects.OrderBy(x => x.Name).Select(x => new ListSubjectItem { Id = x.Name, Name = x.Name }).GroupBy(x => x.Name).Select(g => g.First()).ToList();

            //grade
            var gradeIds = subjects.Select(f => f.GradeId).Distinct().ToList();
            var gradesFinal = gradeService.GetGradesByGradeIdList(gradeIds)
                .OrderBy(x => x.Order)
                .Select(x => new ListItem { Id = x.Id, Name = x.Name })
                .ToList();

            //category
            var categoryIds = new List<int>();
            var bankIds = banksFinal.Select(b => b.Id).ToList();
            if (bankIds.Count() > 2000)
            {
                for (int i = 0; i < Math.Ceiling((double)bankIds.Count() / 2000); i++)
                {
                    var nextIds = bankIds.Skip(i * 2000).Take(2000).ToList();
                    var result = testService.GetValidTestsByBank(nextIds).Select(x => x.DataSetCategoryID.GetValueOrDefault()).ToList();
                    categoryIds.AddRange(result);
                }
            }
            else
            {
                categoryIds = testService.GetValidTestsByBank(bankIds).Select(x => x.DataSetCategoryID.GetValueOrDefault()).ToList();
            }

            var categoriesFinal = _categoriesService.GetCategoriesByListCategoryID(categoryIds.Distinct().ToList())
                .OrderBy(x => x.DataSetCategoryName)
                .Select(x => new ListItem { Id = x.DataSetCategoryID, Name = x.DataSetCategoryName })
                .ToList();

            return new JsonResult()
            {
                Data = new
                {
                    banks = banksFinal,
                    categories = categoriesFinal,
                    subjects = subjectsFinal,
                    grades = gradesFinal
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        [HttpGet]
        public ActionResult GetSubjectsByGradeIdAndAuthorCustomOrder(SearchBankCriteria criteria)
        {
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                criteria.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            criteria.UserId = CurrentUser.Id;
            criteria.UserRole = CurrentUser.RoleId;

            var tmp = subjectService.GetSubjectsByGradeIdAndAuthor(criteria);
            if (tmp != null)
            {
                var subjectIncludes = _manageTestService.GetSubjectIncludes(CurrentUser.Id);
                tmp.AddRange(subjectIncludes);
                tmp = tmp.DistinctBy(m => m.Id).ToList();
                var subjectOrders = subjectService.GetSubjectOrders(criteria.DistrictId);
                if (subjectOrders.Any())
                {
                    var maxOrder = subjectOrders.Max(x => x.Order) + 100;

                    var query = from sj in tmp
                                join order in subjectOrders on sj.Id equals order.SubjectId into ps
                                from p in ps.DefaultIfEmpty()
                                select new { Id= sj.Id, Name = sj.Name, Order = p == null ? maxOrder : p.Order };

                    var orderList = query.GroupBy(s => s.Name).Select(g =>new {Name = g.Key, Order = g.Min(x=>x.Order)}).OrderBy(x => x.Order).ThenBy(x => x.Name);
                    var result = orderList.Select(x => new ListSubjectItem {Id = x.Name, Name = x.Name}).ToList();
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                var item = tmp.GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).OrderBy(x=>x.Name).ToList();
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSubjectsByGradeIdAndAuthor(SearchBankCriteria criteria)
        {
            if ((!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin) || criteria.DistrictId == 0)
                criteria.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            criteria.UserId = CurrentUser.Id;
            criteria.UserRole = CurrentUser.RoleId;

            var tmp = subjectService.GetSubjectsByGradeIdAndAuthor(criteria);
            if (tmp != null)
            {
                var subjectIncludes = _manageTestService.GetSubjectIncludes(CurrentUser.Id);
                tmp.AddRange(subjectIncludes);
                tmp = tmp.DistinctBy(m => m.Id).ToList();
                var item = tmp.GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).OrderBy(x => x.Name).ToList();
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetSubjectsByGradeId(SearchBankCriteria criteria)
        {
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                criteria.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            criteria.UserId = CurrentUser.Id;
            criteria.UserRole = CurrentUser.RoleId;

            var tmp = subjectService.GetSubjectsByGradeIdAndAuthor(criteria);
            if (tmp != null)
            {
                var item = tmp.GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = string.Join(",", x.Select(s => s.Id)), Name = x.Key }).ToList();
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetBanksForItemSetSaveTestNew(string subjectIds, int districtId, string moduleCode = "")
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var subjectIdList = subjectIds.Split(',');
            var data = new List<ListItem>();
            foreach (var subjectId in subjectIdList)
            {
                int id;
                var result = int.TryParse(subjectId, out id);
                if (result)
                {
                    var temp =
                        userBankService.GetBanksForItemSetSaveTest(id, currentDistrictId, CurrentUser.Id,
                            CurrentUser.RoleId).ToList();
                    data.AddRange(temp);
                }
            }

            if (!string.IsNullOrEmpty(moduleCode))
            {
                data = _restrictionBO.FilterBanks(new Models.RestrictionDTO.FilterBankQueryDTO
                {
                    Banks = data,
                    ModuleCode = moduleCode,
                    DistrictId = districtId,
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                });
            }

            data = data.OrderBy(s => s.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSubjectsForItemSetSaveTest(int gradeId, int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var tmp = subjectService.GetSubjectsForItemSetSaveTest(gradeId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId);
            if (tmp != null)
            {
                var item = tmp.GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = string.Join(",", x.Select(s => s.Id)), Name = x.Key }).ToList();
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSubjects(int gradeId, int? districtId)
        {
            var userDistrictId = CurrentUser.IsPublisher() || CurrentUser.IsNetworkAdmin ? districtId : CurrentUser.DistrictId;
            var data = GetSubjects(gradeId, userDistrictId.GetValueOrDefault()).GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = string.Join(",", x.Select(s => s.Id)), Name = x.Key }).ToList();
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
        public ActionResult ACTGetSubjectsNew(int gradeId, int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var tmp = subjectService.ACTGetSubjectsByGradeId(gradeId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId);
            if (tmp != null)
            {
                var item = tmp.GroupBy(s => s.Name).Select(x => new ListSubjectItem() { Id = string.Join(",", x.Select(s => s.Id)), Name = x.Key }).ToList();
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ACTGetBanksNew(string subjectIds, int districtId)
        {
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var data = new List<ListItem>();
            var subjectIdList = subjectIds.Split(',');
            foreach (var id in subjectIdList)
            {
                int subjectId;
                var result = int.TryParse(id, out subjectId);
                if (result)
                {
                    var temp = userBankService.ACTGetUserBanksBySubjectId(subjectId, currentDistrictId, CurrentUser.Id, CurrentUser.RoleId).ToList();
                    data.AddRange(temp);
                }
            }
            data = data.OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCHYTENBankByDistrictId(int districtId)
        {
            var data = new List<ListItem>();

            var districtDecode = districtDecodeService
                .GetDistrictDecodesOfSpecificDistrictByLabel(districtId, "BankStudentTest").FirstOrDefault();
            if (districtDecode != null)
            {
                var bankIds = districtDecode.Value.Split(';').ToList();
                var banks = bankService.Select().Where(x => bankIds.Contains(x.Id.ToString()));

                data = banks.Select(o => new ListItem
                {
                    Id = o.Id,
                    Name = o.Name
                }).ToList();


            }

            data = data.OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
