using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Common.SSO
{
    public static class SSOErrorCode
    {
        private static Dictionary<string, string> _errors = new Dictionary<string, string>
        {
            { "GOOGLE_401", "Your district does not allow to login by Google." },
            { "GOOGLE_500", "Cannot get authorization code." },
            { "GOOGLE_501", "Cannot get access token from authorization code." },
            { "GOOGLE_502", "Cannot get user information from access token." },
            { "GOOGLE_503", "Cannot login to LinkIt." },

            { "CLASSLINK_500", "Cannot get authorization code." },
            { "CLASSLINK_501", "Cannot get access token from authorization code." },
            { "CLASSLINK_502", "Cannot get user information from access token." },
            { "CLASSLINK_503", "Parsing token fail." },
            { "CLASSLINK_400", "Cannot get student information" },
            { "CLASSLINK_401", "Student login but LinkIt user has been found is Staff." },

        };

        public static readonly string GOOGLE_401 = "GOOGLE_401";
        public static readonly string GOOGLE_500 = "GOOGLE_500";
        public static readonly string GOOGLE_501 = "GOOGLE_501";
        public static readonly string GOOGLE_502 = "GOOGLE_502";
        public static readonly string GOOGLE_503 = "GOOGLE_503";

        public static readonly string MICROSOFT_401 = "MICROSOFT_401";
        public static readonly string MICROSOFT_500 = "MICROSOFT_500";
        public static readonly string MICROSOFT_501 = "MICROSOFT_501";
        public static readonly string MICROSOFT_502 = "MICROSOFT_502";
        public static readonly string MICROSOFT_503 = "MICROSOFT_503";

        public static readonly string CLASSLINK_500 = "CLASSLINK_500";
        public static readonly string CLASSLINK_501 = "CLASSLINK_501";
        public static readonly string CLASSLINK_502 = "CLASSLINK_502";
        public static readonly string CLASSLINK_503 = "CLASSLINK_503";
        public static readonly string CLASSLINK_400 = "CLASSLINK_400";
        public static readonly string CLASSLINK_401 = "CLASSLINK_401";

        public static readonly string CANVAS_500 = "CANVAS_500";
        public static readonly string CANVAS_501 = "CANVAS_501";

        public static readonly string CLEVER_501 = "CLEVER_501";
        public static readonly string CLEVER_502 = "CLEVER_502"; // ERROR CODE when clever student role not match student login type linkit
        public static readonly string CLEVER_503 = "CLEVER_503";
        public static string Get(string code)
        {
            return _errors[code];
        }

    }
}
