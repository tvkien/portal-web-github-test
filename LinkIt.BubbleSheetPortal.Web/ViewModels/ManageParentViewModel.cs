using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ManageParentViewModel
    {
        public ManageParentViewModel()
        {
            IsPublisher = false;
            IsNetworkAdmin = false;
            IsDistrictAdmin = false;
            DistrictID = 0;
        }
        public bool IsPublisher { get; set; }
        public bool IsNetworkAdmin { get; set; }
        public bool IsDistrictAdmin { get; set; }
        public int DistrictID { get; set; }
        
        
    }
}