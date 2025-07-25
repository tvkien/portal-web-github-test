using System;
namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    [Serializable]
    public class AccountViewModelBase
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string RedirectUrl { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public bool NoAccount { get; set; }
    }
}
