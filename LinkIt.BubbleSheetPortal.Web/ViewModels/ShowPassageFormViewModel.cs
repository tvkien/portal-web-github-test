using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ShowPassageFormViewModel
    {
        public ShowPassageFormViewModel()
        {
            QTIRefObjectID = 0;
            Qti3pPassageID = 0;
            Qti3pSourceID = 0;
            Is3pItem = false;
            ShownItemXml = string.Empty;
            VirtualTestId = 0;
            VirtualSections = new List<ListItem>();
            IsPublisher = false;
            DistrictId = 0;
        }
        public int QTIRefObjectID { get; set; }
        public int Qti3pPassageID { get; set; }
        public int Qti3pSourceID { get; set; }
        public bool Is3pItem { get; set; }
        public string ShownItemXml { get; set; }
        
        //When importing item for Virtual Test
        public int VirtualTestId { get; set; }
        public List<ListItem> VirtualSections { get; set; }
        public bool IsPublisher { get; set; }
        public int DistrictId { get; set; }
    }
}