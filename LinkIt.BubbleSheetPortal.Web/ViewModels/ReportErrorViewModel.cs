namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ReportErrorViewModel
    {
        public string Comment { get; set; }
        public string Report { get; set; }
        public string Sender { get; set; }
        public string Description { get; set; }
        public bool IsReceived { get; set; }
        public string Message { get; set; }
        public string RecaptchaResponse { get; set; }
    }
}
