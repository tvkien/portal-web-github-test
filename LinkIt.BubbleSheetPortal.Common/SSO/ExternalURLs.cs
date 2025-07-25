
namespace LinkIt.BubbleSheetPortal.Common
{
    public static class ExternalURLs
    {
        public static readonly string CLASSLINK_REQUEST_TOKEN = "https://launchpad.classlink.com/oauth2/v2/token";
        public static readonly string CLASSLINK_REQUEST_AUTH = "https://launchpad.classlink.com/oauth2/v2/auth?scope=profile&redirect_uri={redirect_uri}&client_id={client_id}&response_type=code&state={state}";

        public static readonly string GOOLE_REQUEST_CODE = "https://accounts.google.com/o/oauth2/v2/auth?scope=email&state={state}&redirect_uri={redirect_uri}&access_type=offline&response_type=code&client_id={client_id}";
        public static readonly string GOOGLE_REQUEST_TOKEN = "https://www.googleapis.com/oauth2/v4/token";
        public static readonly string GOOGL_REQUEST_USER_INFO = "https://www.googleapis.com/oauth2/v2/userinfo?alt=json";
        public static readonly string MICROSOFT_REQUEST_CODE = "https://login.microsoftonline.com/{tenantID}/oauth2/v2.0/authorize?client_id={client_id}&scope=openid%20offline_access%20email&response_type=code&redirect_uri={redirect_uri}";
        public static readonly string MICROSOFT_REQUEST_TOKEN = "https://login.microsoftonline.com/{tenantID}/oauth2/v2.0/token";

        public static readonly string MICROSOFT_REQUEST_CODE_MULTITENANT = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id={client_id}&scope=openid%20offline_access%20email&state={state}&response_type=code&redirect_uri={redirect_uri}";
        public static readonly string MICROSOFT_REQUEST_TOKEN_MULTITENANT = "https://login.microsoftonline.com/common/oauth2/v2.0/token";

        public static readonly string CLEVER_REQUEST_CODE = "https://clever.com/oauth/authorize?response_type=code&redirect_uri={redirect_uri}&client_id={client_id}&state={state}";
        public static readonly string CLEVER_REQUEST_TOKEN = "https://clever.com/oauth/tokens";
        public static readonly string CLEVER_GET_TOKEN_INFO = "https://api.clever.com/v3.0/me";
        public static readonly string CLEVER_GET_USER_INFO = "https://api.clever.com/v3.0/users/{0}";
    }
}
