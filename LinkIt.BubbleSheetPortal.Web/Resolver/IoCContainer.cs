using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Resolver
{
    public static class IoCContainer
    {
        public static T GetService<T>(IDependencyResolver resolver) where T : class
        {
            try
            {
                var service = resolver.GetService(typeof(T)) as T;
                return service;
            }
            catch
            {
                return null;
            }
        }
    }
}