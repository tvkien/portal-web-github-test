using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class IUserCookieDataMin
    {
        public int a { get; set; }
        public int b { get; set; }
        public int c { get; set; }
        public int d { get; set; }
        public string e { get; set; }
        public string f { get; set; }
        public string g { get; set; }

        public string h { get; set; }

        public List<int> i { get; set; }
        //For Impersonate update
        public int? j { get; set; }
        public int? k { get; set; }
        public int? l { get; set; }
        public int? m { get; set; }
        public string n { get; set; }
        public string o { get; set; }
        public string p { get; set; }
        public List<int> q { get; set; }
        public string r { get; set; }
        public string s { get; set; }
        public string t { get; set; }

        public int? u { get; set; }//the DistrictID of NetworkAddata when first logging in
        public string v { get; set; }

        public bool w { get; set; }
        // for ensuring single user sign on
        public string x { get; set; }

        public string y { get; set; }

        public UserMetaValue z { get; set; }
        public string a1 { get; set; }
        public string a2 { get; set; }
        public UserWelcomeInfo a3 { get; set; }



        //public int Id { get; set; }
        //public int DId { get; set; }
        //public int SId { get; set; }
        //public int RId { get; set; }
        //public string Name { get; set; }
        //public string Email { get; set; }
        //public string User { get; set; }

        //public string LCode { get; set; }

        //public List<int> DIds { get; set; }
        ////For Impersonate update
        //public int? OId { get; set; }
        //public int? ODId { get; set; }
        //public int? OSId { get; set; }
        //public int? ORId { get; set; }
        //public string OName { get; set; }
        //public string OEmail { get; set; }
        //public string OUser { get; set; }
        //public List<int> ODIds { get; set; }
        //public string ODLCode { get; set; }
        //public string ImLA { get; set; }
        //public string ImSub { get; set; }

        //public int? ONetAdDIds { get; set; }//the DistrictID of NetworkAddata when first logging in
        //public string GUID { get; set; }

        //public bool IsVDETUser { get; set; }
        //// for ensuring single user sign on
        //public string GUIDSession { get; set; }

        //public string CKSession { get; set; }

        //public UserMetaValue Meta { get; set; }
        //public string WM { get; set; }
        //public bool Snippet { get; set; }

        public IUserCookieDataMin()
        {

        }

        public IUserCookieDataMin(IUserCookieData data)
        {
            a = data.ID;
            b  = data.DistrictId;
            c = data.StateId;
            d = data.RoleId;
            e = data.Name;
            f = data.EmailAddress;
            g = data.Username;
            h = data.LocalCode;
            i = data.ListDistrictId;
            j = data.OriginalID;
            k = data.OriginalDistrictId;
            l = data.OriginalStateId;
            m = data.OriginalRoleId;
            n = data.OriginalName;
            o = data.OriginalEmailAddress;
            p = data.OriginalUsername;
            q = data.ListOriginalDistrictId;
            r = data.OriginalDistrictLiCode;
            s = data.ImpersonateLogActivity;
            t = data.ImpersonatedSubdomain;
            u = data.OriginalNetworkAdminDistrictId;
            v = data.SessionCookieGUID;
            w = data.IsVDETUser;
            x = data.GUIDSession;
            y = data.CKSession;
            z = data.UserMetaValue;
           a1  = data.WelcomeMessage;
           a2  = data.WalkmeSnippetURL;
           a3  = data.RoleAndGroupName;
        }
    }
}
