using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    public class HealthCheckController : Controller
    {
        private readonly HealthCheckService healthCheckService;

        public HealthCheckController(HealthCheckService healthCheckService)
        {
            this.healthCheckService = healthCheckService;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var s3Settings = LinkitConfigurationManager.GetS3Settings();
            var result = healthCheckService.Check(s3Settings);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
