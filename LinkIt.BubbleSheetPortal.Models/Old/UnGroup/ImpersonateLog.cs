using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ImpersonateLog 
    {
        public long ImpersonateLogID { get; set; }
        public string SessionCookieGUID { get; set; }
        public string ActionType { get; set; }
        public DateTime ActionTime { get; set; }
        public int? OriginalUserId { get; set; }
        public int CurrentUserId { get; set; }
        public int? ImpersonatedUserId { get; set; }

        public static class ActionTypeEnum
        {
            public static string SignIn = "SignIn";
            public static string SignOut = "SignOut";
            public static string Impersonate = "Impersonate";
            public static string GoBack = "GoBack";
        }
        
    }
}