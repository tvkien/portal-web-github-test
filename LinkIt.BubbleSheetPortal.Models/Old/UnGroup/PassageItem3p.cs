using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class PassageItem3p
    {
        public int QTI3pPassageID { get; set; }
        public string Source { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public int GradeID { get; set; }
        public string GradeName { get; set; }
        public string TextType { get; set; }
        public string WordCound { get; set; }
        public string TextSubType { get; set; }
        public string FleschKinkaidName { get; set; }
        public string PassageType { get; set; }
        public string PassageGenre { get; set; }
        public int? Lexile { get; set; }
        public string Spache { get; set; }
        public string DaleChall { get; set; }
        public string RMM { get; set; }
        public int ItemsMatchCount { get; set; }
        public string ItemsMatchXml { get; set; }
        public int ItemsAllCount { get; set; }
        public string ItemsAllXml { get; set; }
        public int TotalRow { get; set; }

    }

    public class PassageItem3pFromItemLibrary
    {
        public int QTI3pPassageID { get; set; }
        public string Source { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public int GradeID { get; set; }
        public string GradeName { get; set; }
        public string TextType { get; set; }
        public string WordCound { get; set; }
        public string TextSubType { get; set; }
        public string FleschKinkaidName { get; set; }
        public string PassageType { get; set; }
        public string PassageGenre { get; set; }
        public int? Lexile { get; set; }
        public string Spache { get; set; }
        public string DaleChall { get; set; }
        public string RMM { get; set; }
        public int ItemsAllCount { get; set; }
        public string ItemsAllXml { get; set; }
        public int TotalRow { get; set; }

    }
}
