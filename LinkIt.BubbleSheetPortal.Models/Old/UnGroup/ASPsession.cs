using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ASPSession
    {
        public int ASPSessionTokenID { get; set; }
        public string SessionToken { get; set; }
        public System.DateTime expires { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int? LoginID { get; set; }
        public string CKSession { get; set; }
    }
}
