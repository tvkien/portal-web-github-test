using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class PassageItem
    {
        public int QTIRefObjectID { get; set; }
        public string Source { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string GradeName { get; set; }
        public string TextType { get; set; }
        public string TextSubType { get; set; }
        public string FleschKinkaidName { get; set; }
        public int ItemsMatchCount { get; set; }
        public string ItemsMatchXml { get; set; }
        public int ItemsAllCount { get; set; }
        public string ItemsAllXml { get; set; }
        public int TotalRow { get; set; }
        public bool? HasQTI3pPassage { get; set; }
        public int CreatedBy { get; set; }
        public int? DistrictID { get; set; }
        public bool CanEdit { get; set; }
    }

    public class PassageItemFromItemLibrary
    {
        public int QTIRefObjectID { get; set; }
        public string Source { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string GradeName { get; set; }
        public string TextType { get; set; }
        public string TextSubType { get; set; }
        public string FleschKinkaidName { get; set; }
        public int ItemsAllCount { get; set; }
        public string ItemsAllXml { get; set; }
        public int TotalRow { get; set; }
        public bool? HasQTI3pPassage { get; set; }

    }
}
