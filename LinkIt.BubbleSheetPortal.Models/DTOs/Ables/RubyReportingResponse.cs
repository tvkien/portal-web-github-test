namespace LinkIt.BubbleSheetPortal.Models.DTOs.Ables
{
    public class RubyReportingResponse
    {
        public RubyReportingResult Result { get; set; }

        public string TargetUrl { get; set; }

        public bool Success { get; set; }

        public string Error { get; set; }

        public bool UnAuthorizedRequest { get; set; }
    }
}
