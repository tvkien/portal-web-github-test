using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.App_Start
{
    public class JsonNetFilterAttribute : System.Web.Mvc.ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is JsonResult == false)
                return;

            filterContext.Result = new CustomJsonResult((JsonResult)filterContext.Result);
        }

        private class CustomJsonResult : JsonResult
        {
            public CustomJsonResult(JsonResult jsonResult)
            {
                ContentEncoding = jsonResult.ContentEncoding;
                ContentType = jsonResult.ContentType;
                Data = jsonResult.Data;
                JsonRequestBehavior = jsonResult.JsonRequestBehavior;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet
                    && String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("GET not allowed! Change JsonRequestBehavior to AllowGet.");

                var response = context.HttpContext.Response;

                response.ContentType = String.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

                if (this.ContentEncoding != null)
                    response.ContentEncoding = this.ContentEncoding;

                if (this.Data != null)
                {
                    var json = JsonConvert.SerializeObject(
                        this.Data,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });

                    response.Write(json);
                }
            }
        }
    }
}
