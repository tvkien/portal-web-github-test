using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Plus.v1;
using Google.Apis.Util.Store;
using System;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class AppFlowMetadata : FlowMetadata
    {
        private static readonly IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "393350479322-l3lqa5vsv413loha2nt67skk9er9n14u.apps.googleusercontent.com",
                    ClientSecret = "FsvOFvnmSmU511VoNpkTqupp"
                },
                Scopes = new[] { PlusService.Scope.UserinfoEmail },
                DataStore = new FileDataStore("Drive.Api.Auth.Store"),

            });

        public override string GetUserId(Controller controller)
        {
            // In this sample we use the session to store the user identifiers.
            // That's not the best practice, because you should have a logic to identify
            // a user. You might want to use "OpenID Connect".
            // You can read more about the protocol in the following link:
            // https://developers.google.com/accounts/docs/OAuth2Login.
            var user = controller.Session["user"];
            if (user == null)
            {
                user = Guid.NewGuid();
                controller.Session["user"] = user;
            }
            return user.ToString();

        }

        public override IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }

		public override string AuthCallback
		{
			get
			{
				return @"/AuthCallback/IndexAsync";
			}
		}
	}
}
