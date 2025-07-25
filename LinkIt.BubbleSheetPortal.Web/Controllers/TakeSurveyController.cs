using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class TakeSurveyController : BaseController
    {
        private QTITestClassAssignmentService _qtiTestClassAssignmentService;
        private readonly StudentMetaService _studentMetaServices;

        public TakeSurveyController(QTITestClassAssignmentService qtiTestClassAssignmentService,
            StudentMetaService studentMetaService)
        {
            _qtiTestClassAssignmentService = qtiTestClassAssignmentService;
            _studentMetaServices = studentMetaService;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TakeSurveys)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetQtiAssignment()
        {
            var parser = new DataTableParser<QTITestClassAssignmentForSurveyDto>();            
            var studentId = _studentMetaServices.GetStudentIdViaStudentUser(CurrentUser.Id);
            var roleId = CurrentUser.RoleId;
            var userId = CurrentUser.Id;
            IEnumerable<QTITestClassAssignmentForSurveyDto> assignments = _qtiTestClassAssignmentService.GetForSurvey(userId, roleId, studentId);

            return Json(parser.Parse(assignments.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }
    }
}
