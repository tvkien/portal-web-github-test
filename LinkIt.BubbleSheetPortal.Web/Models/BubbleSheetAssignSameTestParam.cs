using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Models
{
    public class BubbleSheetAssignSameTestParam
    {
        public List<string> StudentIdList { get; set; }
        public int ClassId { get; set; }
        public int TestId { get; set; }
        public int GroupId { get; set; }
        public bool IsGenericBubbleSheet { get; set; }
        public bool IsGroupPrinting { get; set; }
        public string TestName { get; set; }
        public string SSearch { get; set; }
    }
}
