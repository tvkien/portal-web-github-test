using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Models;
using System.Linq;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using System;
using LinkIt.BubbleSheetPortal.Web.Constant;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class NotificationMessageController : BaseController
    {
        private readonly NotificationMessageControllerParameters parameters;
        
        public NotificationMessageController(NotificationMessageControllerParameters parameters)
        {
            this.parameters = parameters;
        }

        public ActionResult GetNotification(int currentIndex)
        {
            var notificationMessages = new List<NotificationMessage>();

            if (CurrentUser.UserMetaValue != null)
            {
                var takeItem = 5;

                if (currentIndex == 0)
                {
                    takeItem = 10;
                    CurrentUser.UserMetaValue.LatestNotificationClicked = DateTime.UtcNow;
                    parameters.FormsAuthenticationService.RefreshFormsAuthCookie(CurrentUser, false, false);

                    SaveUserMetaInformation();
                }

                var isGetFromDatabase = currentIndex == 0;
                notificationMessages = GetNotificationMessages(isGetFromDatabase)
                    .Skip(currentIndex).Take(takeItem).ToList();
            }

            return Json(notificationMessages, JsonRequestBehavior.AllowGet);
        }

        private void SaveUserMetaInformation()
        {
            var userMeta = parameters.UserMetaService.GetByUserId(CurrentUser.Id, UserMetaLabelConst.NOTIFICATION);
            userMeta.UserMetaValue = CurrentUser.UserMetaValue;
            parameters.UserMetaService.Save(userMeta);
        }

        private List<NotificationMessage> GetNotificationMessages(bool isGetFromDatabase)
        {
            var key = "NotificationMessage_" + CurrentUser.DistrictId;

            if(isGetFromDatabase && CacheHelper.Exists(key))
            {
                CacheHelper.Clear(key);
            }

            if (!CacheHelper.Exists(key))
            {
                CacheHelper.Add(parameters.NotificationMessageService.GetByDistrictId(CurrentUser.DistrictId.GetValueOrDefault(), CurrentUser.Id), key);
            }

            List<NotificationMessage> notificationMessages;
            CacheHelper.Get(key, out notificationMessages);
            return notificationMessages;
        }

        public ActionResult LoadUnreadMessage()
        {

            var unreadMessageNo = 0;

            if (CurrentUser.UserMetaValue != null)
            {
                unreadMessageNo = GetNotificationMessages(true).Count(x => x.PublishedTime > CurrentUser.UserMetaValue.LatestNotificationClicked);
            }

            ViewBag.UnreadMessageNo = unreadMessageNo;
            return View();
        }
    }
}
