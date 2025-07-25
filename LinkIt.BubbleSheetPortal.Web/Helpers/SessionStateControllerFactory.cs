using System;
using System.Web.Mvc;
using System.Web.SessionState;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class SessionStateControllerFactory : DefaultControllerFactory
    {
        protected override SessionStateBehavior GetControllerSessionBehavior(
            System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null || !Attribute.IsDefined(controllerType, typeof (SessionStateAttribute)))
            {

                return SessionStateBehavior.ReadOnly;

            }

            // Use the default method to look up the actual value of the session attribute. It uses a cache for lookup
            return base.GetControllerSessionBehavior(requestContext, controllerType);
        }
    }
}