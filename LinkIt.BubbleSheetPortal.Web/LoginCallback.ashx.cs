namespace LinkIt.BubbleSheetPortal.Web
{
    using System;
    using System.Threading.Tasks;
    using Auth0.AuthenticationApi;
    using Auth0.AuthenticationApi.Models;
    using System.Configuration;
    using System.Web;

    public class LoginCallback : HttpTaskAsyncHandler, System.Web.SessionState.IRequiresSessionState
    {
        public override async Task ProcessRequestAsync(HttpContext context)
        {
            AuthenticationApiClient client = new AuthenticationApiClient(
                new Uri(string.Format("https://{0}", ConfigurationManager.AppSettings["auth0:Domain"])));

            var returnTo = "/Account/SSOCallback";
            try
            {
                var token = await client.GetTokenAsync(new AuthorizationCodeTokenRequest
                {
                    ClientId = context.Session["Auth0ClientId"] != null ? context.Session["Auth0ClientId"].ToString() : string.Empty,
                    ClientSecret = context.Session["Auth0ClientSecret"] != null ? context.Session["Auth0ClientSecret"].ToString() : string.Empty,
                    Code = context.Request.QueryString["code"],
                    RedirectUri = context.Request.Url.ToString()
                });

                var profile = await client.GetUserInfoAsync(token.AccessToken);
                context.Session.Add("SSOInfo", profile.NickName);

                var state = context.Request.QueryString["state"];
                if (state != null)
                {
                    var stateValues = HttpUtility.ParseQueryString(context.Request.QueryString["state"]);
                    var redirectUrl = stateValues["ru"];

                    // check for open redirection
                    if (redirectUrl != null && IsLocalUrl(redirectUrl))
                    {
                        context.Session.Add("ReturnURL", redirectUrl);
                    }
                }

                context.Response.Redirect(returnTo);
            }
            catch (Exception)
            {
                context.Response.Redirect(returnTo);
            }
            
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private bool IsLocalUrl(string url)
        {
            return !String.IsNullOrEmpty(url)
                && url.StartsWith("/")
                && !url.StartsWith("//")
                && !url.StartsWith("/\\");
        }
    }
}