namespace LinkIt.BubbleSheetPortal.Web.Models.ParentConnect
{
    public class ComposeModel
    {
        public ComposeModel()
        {
            IsCompose = false;
            Sendby = string.Empty;
            SelectedStudentGroupId = string.Empty;
            SelectedStudentId = string.Empty;
            ClassId = string.Empty;
            From = string.Empty;
            Subject = string.Empty;
            HtmlContent = string.Empty;
            UseEmail = string.Empty;
            UseSms = string.Empty;
            UseRequestAck = string.Empty;
            ReplyEnable = string.Empty;
        }
        public bool IsCompose { get; set; }
        public string Sendby { get; set; }
        public string SelectedStudentGroupId { get; set; }
        public string SelectedStudentId { get; set; }
        public string ClassId { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string HtmlContent { get; set; }
        public string UseEmail { get; set; }
        public string UseSms { get; set; }
        public string UseRequestAck { get; set; }
        public string ReplyEnable { get; set; }        
    }
}