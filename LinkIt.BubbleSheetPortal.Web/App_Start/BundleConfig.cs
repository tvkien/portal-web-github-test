using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using LinkIt.BubbleSheetPortal.Web.Helpers;

namespace LinkIt.BubbleSheetPortal.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles()
        {
            var bundlehelper = typeof(BundleHelper);
            var allMethods = bundlehelper.GetMethods();
            foreach (MethodInfo bundleMethod in allMethods)
            {
                if(Attribute.GetCustomAttributes(bundleMethod).Any(x=>x.GetType() == typeof(BundleAppStartMethodAttribute)))
                {
                    bundleMethod.Invoke(null, null);
                }
            }
        }
    }
}