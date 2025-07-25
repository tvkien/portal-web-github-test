using System;
using System.Collections.Generic;
using System.Web.Mvc;
using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class ProfiledDependencyResolver : IDependencyResolver
    {
        private readonly IDependencyResolver resolver;

        public ProfiledDependencyResolver(IDependencyResolver resolver)
        {
            this.resolver = resolver;
        }

        public object GetService(Type serviceType)
        {
            using (MiniProfiler.Current.Step("GetService+" + serviceType.FullName))
            {
                return resolver.GetService(serviceType);
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            using (MiniProfiler.Current.Step("GetServices+" + serviceType.FullName))
            {
                return resolver.GetServices(serviceType);
            }
        }
    }
}
