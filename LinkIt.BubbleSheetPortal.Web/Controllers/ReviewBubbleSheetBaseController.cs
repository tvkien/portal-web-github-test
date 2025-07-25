using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using LinkIt.BubbleService.Shared.Data;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using RestSharp.Extensions;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetReview;
using LinkIt.BubbleSheetPortal.Web.Security;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    public class ReviewBubbleSheetBaseController : BaseController
    {
        private readonly BubbleSheetListService bubbleSheetListService;
        private readonly BubbleSheetService bubbleSheetService;
        private readonly VulnerabilityService vulnerabilityService;
        public readonly UserService userService;
        public readonly DistrictDecodeService districtDecodeService;
        public ReviewBubbleSheetBaseController(BubbleSheetListService bubbleSheetListService,
            BubbleSheetService bubbleSheetService,
            VulnerabilityService vulnerabilityService,
            UserService userService,
            DistrictDecodeService districtDecodeService)
        {
            this.bubbleSheetListService = bubbleSheetListService;
            this.bubbleSheetService = bubbleSheetService;
            this.vulnerabilityService = vulnerabilityService;
            this.userService = userService;
            this.districtDecodeService = districtDecodeService;
        }

        protected IQueryable<BubbleSheetReviewListViewModel> GetBubbleSheetReviewListData(int districtId, bool archived)
        {
            int vDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            vDistrictId = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin ? districtId : vDistrictId;

            var result = bubbleSheetListService.GetBubbleSheetListProc(vDistrictId, CurrentUser.Id, CurrentUser.RoleId, archived).Select(x => new BubbleSheetReviewListViewModel
            {
                Ticket = x.Ticket,
                Grade = x.GradeName,
                //Subject = x.SubjectName,
                //Bank = x.BankName,
                Test = x.TestName,
                Class = x.ClassName,
                Teacher = x.TeacherName,
                DateCreated = x.DateCreated,
                DateUpdated = x.DateUpdated,
                GradedCount = x.GradedCount + " / " + x.TotalSheets,
                ClassId = x.ClassId,
                IsArchived = x.IsArchived,
                IsDownloadable = !x.IsManualEntry,
                UnmappedSheetCount = x.UnmappedCount,
                VirtualTestSubTypeID = x.VirtualTestSubTypeID ?? 1
            });
            return result;
        }

        protected IQueryable<BubbleSheetReviewListViewModel> GetBubbleSheetReviewListData(int districtId, int schoolId, bool archived, DateTime dateFilter)
        {
            int vDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            vDistrictId = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin ? districtId : vDistrictId;

            var result = bubbleSheetListService.GetBubbleSheetListProcV2(vDistrictId,
                schoolId,
                CurrentUser.Id,
                CurrentUser.RoleId,
                archived,
                dateFilter)
                .Select(x => new BubbleSheetReviewListViewModel
                {
                    BankID = x.BankID,
                    Ticket = x.Ticket,
                    Grade = x.GradeName,
                    Subject = x.SubjectName,
                    Bank = x.BankName,
                    Test = x.TestName,
                    Class = x.ClassName,
                    Teacher = x.ClassId > 0 ? x.TeacherName : ParseGroupTeacherName(x.GroupTeacherName),
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    GradedCount = x.GradedCount + " / " + x.TotalSheets,
                    ClassId = x.ClassId,
                    IsArchived = x.IsArchived,
                    IsDownloadable = !x.IsManualEntry,
                    UnmappedSheetCount = x.UnmappedCount,
                    VirtualTestSubTypeID = x.VirtualTestSubTypeID ?? 1
                });
            return result;
        }

        protected GetBubbleSheetReviewResponse GetBubbleSheetReviewListDataIncludedStatus(GetBubbleSheetReviewRequest request)
        {
            var response = bubbleSheetListService.GetBubbleSheetListProc_IncludedStatus(request);
            return response;
        }

        protected string ParseGroupTeacherName(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return "";

            var xdoc = XDocument.Parse(xml);
            var result = new List<string>();

            foreach (var node in xdoc.Element("ItemList").Elements("Item"))
            {
                result.Add(GetStringValue(node.Element("FullName")));
            }

            string groupTeacherName = "";

            if (result.Count > 0)
            {
                groupTeacherName += result[0];

                for (int i = 1; i < result.Count; i++)
                {
                    groupTeacherName += "<br>" + result[i];
                }
            }

            return groupTeacherName;
        }

        private string GetStringValue(XElement element)
        {
            if (element == null) return string.Empty;
            return element.Value;
        }



        protected string GetImageUrl<T>(string outputFileName) where T : IFileBlob
        {
            if (string.IsNullOrEmpty(outputFileName))
            {
                return string.Empty;
            }

            var storageContext = DependencyResolver.Current.GetService<IStorageContext<T>>();
            return storageContext.GetPublicReadUrl(outputFileName, TimeSpan.FromMinutes(30));
        }

        protected bool CheckUserCanAccessClass(int userID, int roleID, int classID)
        {
            return vulnerabilityService.CheckUserPermissionOnClassOrSchool(userID, roleID, classID, "Class");
        }
        protected bool CheckUserCanAccessClass(int userID, int roleID, string ticket)
        {
            try
            {
                var obj = bubbleSheetService.GetBubbleSheetByTicket(ticket).FirstOrDefault(o => o.ClassIds != string.Empty && (!o.ClassId.HasValue || o.ClassId == 0));
                if (obj != null)
                {
                    string[] arr = obj.ClassIds.Split(';');
                    if (arr.Length > 0)
                    {
                        int classId = 0;
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (int.TryParse(arr[i], out classId))
                            {
                                if (vulnerabilityService.CheckUserPermissionOnClassOrSchool(userID, roleID, classId, "Class"))
                                    return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return false;
            }
        }       
    }
}
