namespace LinkIt.BubbleSheetPortal.Models
{
    public class LTIInformation
    {
        public int LTIInformationID { get; set; }

        public string PlatformID { get; set; }

        public string ClientID { get; set; }

        public string DeploymentID { get; set; }

        public string AuthorizationServerID { get; set; }

        public string AuthenticationRequestURL { get; set; }

        public string AccessTokenURL { get; set; }

        public string PublicKey { get; set; }

        public int DistrictID { get; set; }
    }
}
