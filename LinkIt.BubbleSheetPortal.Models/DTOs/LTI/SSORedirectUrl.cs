namespace LinkIt.BubbleSheetPortal.Models.DTOs.LTI
{
    public class SSORedirectUrl
    {
        public int SSORedirectUrlId { get; set; }
        public int SSOInformationId { get; set; }
        public string RedirectUrl { get; set; }
        public int RoleId { get; set; }
        public string Type { get; set; }
        public string XLIModuleCode { get; set; }
    }
}
