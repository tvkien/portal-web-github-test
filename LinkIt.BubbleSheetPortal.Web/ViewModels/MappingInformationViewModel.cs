using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class MappingInformationViewModel
    {
        public int ProgressStatus { get; set; }
        public string Name {get;set;}
        public int LoaderType { get; set; }
        public DateTime CreatedDate { get; set; }
        public int MapID { get; set; }
    }
}