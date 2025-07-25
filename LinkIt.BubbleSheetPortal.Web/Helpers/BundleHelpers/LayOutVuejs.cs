using SquishIt.Framework;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.BundleHelpers
{
    public partial class BundleHelperVuejs
    {
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSharedBundle()
        {
            return MvcHtmlString.Create(
                   Bundle.Css()
                    .Add("~/Content/themes/Constellation/css/reset.css")
                    .Add("~/Content/themes/Constellation/css/common.css")
                    .Add("~/Content/themes/Constellation/css/custom.css")
                    .Add("~/Content/themes/Constellation/css/standard.css")
                    .Add("~/Content/themes/Constellation/css/special-pages.css")
                    .Add("~/Content/themes/Constellation/css/960.gs.css")
                    .Add("~/Content/themes/Constellation/css/table.css")
                    .Add("~/Content/themes/Constellation/css/form.css")
                    .Render("/Content/combined/css/Common_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptSharedBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("/Content/themes/Constellation/js/old-browsers.js")
                .Add("/Content/themes/Constellation/js/common.js")
                .Add("/Content/themes/Constellation/js/standard.js")
                .Add("/Content/themes/Constellation/js/jquery.tip.js")
                .Add("/Content/themes/Constellation/js/jquery.hashchange.js")
                .Add("/app/js/layout/index.js")
                .Add("/app/js/layout/notification.js")
                .Add("/Scripts/jquery.dataTables-1.9.4.js")
                .Render("/Content/combined/script/SharedLayout_#.js")
                );
        }
    }
}
