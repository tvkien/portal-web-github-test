using System.Collections.Generic;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class SessionManager
    {
        public static int LoginCount
        {
            get
            {
                var value = HttpContext.Current.Session["LoginCount"];
                if (value == null)
                {
                    return 0;
                }
                else
                {
                    return (int) value;
                }
            }
            set { HttpContext.Current.Session["LoginCount"] = value; }
        }
        public static int ResetPwCount
        {
            get
            {
                var value = HttpContext.Current.Session["ResetPwCount"];
                if (value == null)
                {
                    return 0;
                }
                else
                {
                    return (int)value;
                }
            }
            set { HttpContext.Current.Session["ResetPwCount"] = value; }
        }
        public static bool ShowCaptcha
        {
            get
            {
                var value = HttpContext.Current.Session["ShowCaptcha"];
                if (value == null)
                {
                    return false;
                }
                else
                {
                    return (bool)value;
                }
            }
            set { HttpContext.Current.Session["ShowCaptcha"] = value; }
        }
        public static List<int> ListDistrictId
        {
            get
            {
                return (List<int>)HttpContext.Current.Session["ListDistrictId"]; 
            }
            set
            {
                if (value == null)
                {
                    value = new List<int>();
                }
                HttpContext.Current.Session["ListDistrictId"] = value;
            }
        }
    }
}
