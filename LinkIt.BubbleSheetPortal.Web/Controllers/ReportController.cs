using System;
using System.Linq;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AdminOnly, AjaxAwareAuthorize]
    public class ReportController : BaseController
    {
        private readonly ReportService service;
        private readonly IValidator<Report> validator;

        public ReportController(ReportService service, IValidator<Report> validator)
        {
            this.service = service;
            this.validator = validator;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TechMgmtReports)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetReports()
        {
            var reports = service.GetAllReports().Select(x => new ReportViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    DateCreated = x.DateCreated,
                    URL = x.URL
                });
            var parser = new DataTableParser<ReportViewModel>();
            return Json(parser.Parse(reports), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddNewReport()
        {
            return PartialView();
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddNewReport(Report model)
        {
            model.SetValidator(validator);

            if(!model.IsValid)
            {
                return Json(new {ErrorList = model.ValidationErrors, success = false}, JsonRequestBehavior.AllowGet);
            }

            model.DateCreated = DateTime.UtcNow;
            service.Insert(model);
            return Json(new {success = true}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditReport(int reportId)
        {
            var report = service.GetReportById(reportId);
            if(report.IsNull())
            {
                return HttpNotFound();
            }

            return PartialView(report);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult EditReport(Report model)
        {
            var report = service.GetReportById(model.Id);
            if (report.IsNull())
            {
                model.SetValidator(validator);
                return ShowJsonResultException(model, "An error has occured. Please refresh the page and try again.");
            }

            report.Name = model.Name;
            report.URL = model.URL;
            report.SetValidator(validator);

            if (!report.IsValid)
            {
                return Json(new { ErrorList = report.ValidationErrors, success = false }, JsonRequestBehavior.AllowGet);
            }

            service.Update(report);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteReport(int reportId)
        {
            var report = service.GetReportById(reportId);
            if (report.IsNull())
            {
                return HttpNotFound();
            }

            service.Delete(report);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}