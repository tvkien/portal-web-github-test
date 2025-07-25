using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class IUserCookieData
    {
        public int ID { get; set; }
        public int DistrictId { get; set; }
        public int StateId { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Username { get; set; }

        public string LocalCode { get; set; }

        public List<int> ListDistrictId { get; set; }
        //For Impersonate update
        public int? OriginalID { get; set; }
        public int? OriginalDistrictId { get; set; }
        public int? OriginalStateId { get; set; }
        public int? OriginalRoleId { get; set; }
        public string OriginalName { get; set; }
        public string OriginalEmailAddress { get; set; }
        public string OriginalUsername { get; set; }
        public List<int> ListOriginalDistrictId { get; set; }
        public string OriginalDistrictLiCode { get; set; }
        public string ImpersonateLogActivity { get; set; }
        public string ImpersonatedSubdomain { get; set; }

        public int? OriginalNetworkAdminDistrictId { get; set; }//the DistrictID of NetworkAdmin when first logging in
        public string SessionCookieGUID { get; set; }

        public bool IsVDETUser { get; set; }
        // for ensuring single user sign on
        public string GUIDSession { get; set; }

        public string CKSession { get; set; }

        public UserMetaValue UserMetaValue { get; set; }
        public string WelcomeMessage { get; set; }
        public string WalkmeSnippetURL { get; set; }
        public UserWelcomeInfo RoleAndGroupName { get; set; }


        public IUserCookieData()
        {

        }
        public IUserCookieData(IUserCookieDataMin min)
        {
            ID = min.a;
            DistrictId = min.b;
            StateId = min.c;
            RoleId = min.d;
            Name = min.e;
            EmailAddress = min.f;
            Username = min.g;
            LocalCode = min.h;
            ListDistrictId = min.i;
            OriginalID = min.j;
            OriginalDistrictId = min.k;
            OriginalStateId = min.l;
            OriginalRoleId = min.m;
            OriginalName = min.n;
            OriginalEmailAddress = min.o;
            OriginalUsername = min.p;
            ListOriginalDistrictId = min.q;
            OriginalDistrictLiCode = min.r;
            ImpersonateLogActivity = min.s;
            ImpersonatedSubdomain = min.t;
            OriginalNetworkAdminDistrictId = min.u;
            SessionCookieGUID = min.v;
            IsVDETUser = min.w;
            GUIDSession = min.x;
            CKSession = min.y;
            UserMetaValue = min.z;
            WelcomeMessage = min.a1;
            WalkmeSnippetURL = min.a2;
            RoleAndGroupName = min.a3;
        }
    }
}
